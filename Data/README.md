## Data
This part contains the data and scripts used to generate the tables and figures in the paper. It also includes a symbolic link to a folder that contains the outputs of every LLM agent for every experiment listed in the paper.

## Scripts for figure generation 
### gnuplot scripts
Install `gnuplot` in your system.
Run the commands below to analyze data and to generate figures from provided `.plt` files.

```bash
# General
gnuplot "<path_to_plt_file>"

# Example
gnuplot "figure/gnuplot/fig3a/optimization.plt"
```

### Python scripts
Run the commands below to generate figures from provided Python scripts. You may need to install required Python packages by yourself.

```bash
# Change to the figure/python directory
cd figure/python
# install the required dependencies
pip install -r requirements.txt
# Change to specific figure directory
cd "<figure_directory>"
python generate.py

```

## Scripts for data analysis
### Python scripts
Run the commands below to analyze raw data from experiments. You may need to install required Python packages by yourself.

```bash
# General
python "<path_to_python_script>"

# Example
python "python/analysis/python/analyze-g2/analyze.py"
```

## Agent outputs
Check the `examples` folder for the outputs of every LLM agent for every experiment listed in the paper.
