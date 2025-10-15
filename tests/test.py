
import sys
import os
import subprocess

test_dir = os.path.dirname(os.path.realpath(__file__))
root_dir = os.path.realpath(f"{test_dir}/../")

print(f"root directory - {root_dir}")
print(f"test directory - {test_dir}")

def run(app_args):
    if os.path.exists(f'{root_dir}/release/ZMake.dll'):
        cmd = ['dotnet', f'{root_dir}/release/ZMake.dll', '--']
    else:
        prefix = ''
        if sys.platform.startswith('win'):
            prefix = '.exe'
        cmd = [f'{root_dir}/release/ZMake{prefix}']

    subprocess.run(cmd + app_args, check=True)

run(['--help'])
run(['--version'])
