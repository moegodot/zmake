//! # Transformer Example
//!
//! This example demonstrates code transformation using the Oxc transformer.
//! It supports various transformation options including Babel compatibility
//! and environment-specific transforms.
//!
//! ## Usage
//!
//! Create a `test.js` file and run:
//! ```bash
//! cargo run -p oxc_transformer --example transformer [filename] [options]
//! ```
//!
//! ## Options
//!
//! - `--babel-options <path>`: Path to Babel options file
//! - `--targets <targets>`: Browser/environment targets
//! - `--target <target>`: Single target environment

use std::path::Path;

use oxc::allocator::Allocator;
use oxc::codegen::Codegen;
use oxc::parser::Parser;
use oxc::semantic::SemanticBuilder;
use oxc::span::SourceType;
use oxc::transformer::{TransformOptions, Transformer};
use std::ffi::CStr;
use std::ffi::CString;

// Instruction:
// create a `test.js`,
// run `just example transformer` or `just watch-example transformer`

/// Demonstrate code transformation with various options
#[unsafe(no_mangle)]
pub extern "C" fn transform_typescript(
    source_code_ptr: *const i8,
    source_name_ptr: *const i8,
) -> *mut i8 {
    let allocator = Allocator::new();

    let source_code_raw = unsafe { CStr::from_ptr(source_code_ptr) };
    let source_code = source_code_raw.to_str().unwrap();

    let source_name_raw = unsafe { CStr::from_ptr(source_name_ptr) };
    let source_name = source_name_raw.to_str().unwrap();

    let path = Path::new(source_name);

    let options = TransformOptions::from_target("es2023").unwrap();

    let ret = Parser::new(&allocator, &source_code, SourceType::ts()).parse();

    let transformer = Transformer::new(&allocator, &path, &options);

    let mut program = ret.program;

    let ret = SemanticBuilder::new()
        // Estimate transformer will triple scopes, symbols, references
        .with_excess_capacity(2.0)
        .build(&program);

    let ret = transformer.build_with_scoping(ret.semantic.into_scoping(), &mut program);

    if !ret.errors.is_empty() {
        println!("Transformer Errors at `{}`:", source_name);
        for error in ret.errors {
            let error = error.with_source_code(source_code);
            println!("{error:?}");
        }
    }

    let ret = CString::new(Codegen::new().build(&program).code).expect("CString::new failed");

    return ret.into_raw();
}

#[unsafe(no_mangle)]
pub extern "C" fn free_transformed_typescript(generated: *mut i8) {
    unsafe { drop(CString::from_raw(generated)) }
}
