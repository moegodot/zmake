/* eslint-env browser */

level = __Z_MAKE_COLOR ? 3 : 0

const colorSupport = level !== 0 && {
	level,
	hasBasic: true,
	has256: level >= 2,
	has16m: level >= 3,
};

const supportsColor = {
	stdout: colorSupport,
	stderr: colorSupport,
};

export default supportsColor;
