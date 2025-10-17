
import sys
import os
import subprocess
import shutil

root_dir = os.path.dirname(os.path.realpath(__file__))
test_dir = os.path.abspath(f"{root_dir}/tests/")
python = shutil.which("python3")

mode = sys.argv[1] # release or debug
aot = sys.argv[2] # enable or disable
engine = sys.argv[3] # jint or clear-script

if python is None:
    python = shutil.which("python")

print(f"root directory - {root_dir}")
print(f"test directory - {test_dir}")
print(f"use python - {python}")

def run_zmake(app_args):
    #if os.path.exists(f'{root_dir}/release/ZMake.dll'):
    #    cmd = ['dotnet', f'{root_dir}/release/ZMake.dll', '--']
    #else:
    prefix = ''
    if sys.platform.startswith('win'):
        prefix = '.exe'
    cmd = [f'{root_dir}/release/ZMake{prefix}']

    subprocess.run(cmd + app_args, check=True)

subprocess.run([python,f"{root_dir}/build.py",mode,"auto","auto",aot],check=True)

run_zmake(['--help'])
run_zmake(['--version'])
run_zmake(['--verbose',"--script-mode",f"{test_dir}/test.ts", f"--use-{engine}"])
