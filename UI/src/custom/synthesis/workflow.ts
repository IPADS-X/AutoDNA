export const workflow_chat = [
    {
        role: "user",
        content: "生成一个合成方案。",
    },
    {
        role: "assistant",
        content: "# 我搜索到了以下文献并生成如下工艺：\n1. 《Enzymatic DNA Synthesis by Engineering Terminal Deoxynucleotidyl Transferase》\n2. 《Dynabeads™ M-270 Streptavidin》\n3. 《Using Untagged Aminoxy Reversible Terminators with DNA Polymerases》\n# 酶催化脱新DNA合成的改进溶液配置方案\n\n1. 反应缓冲液（10x浓缩液）\n\n- **组件**：\n\n\t- **500 mM磷酸盐缓冲液（pH 6.8）**：通过蒸发至1 mL来预浓缩5 mL 100 mM磷酸盐缓冲溶液。\n\n\t- **1 M NaCl**：将0.333 mL 3M NaCl与0.667 mL水混合。\n\n\t- **2.5 mM氯化钴**：加入0.25 mL 10 mM CoCl₂。\n\n\t- **添加剂（在1x稀释过程中添加）**：\n\n\t\t- **0.1%吐温-20**：每10 mL 1x反应缓冲液中加入100µL 1%吐温-20。\n\n2. 洗涤缓冲液\n\n- **50 mM磷酸盐（pH 6.8）、100 mM氯化钠、0.1%吐温-20**：\n\n- 将**5 mL 100 mM磷酸缓冲液**、**0.333 mL 3M NaCl**、**1 mL 1%吐温-20**混合，用水调节至10 mL。\n\n\n3. 脱保护缓冲液\n\n- **NaNO₂缓冲液**：\n\n- **700 mM NaNO₂溶于1 M乙酸钠（pH 5）**：\n\n- 将2.33 mL 300 mM NaNO₂浓缩至1 mL（700 mM）。\n\n- 将3.33 mL 300 mM乙酸钠浓缩至1 mL（1 M）。\n\n- 混合等体积（各0.5 mL），用HCl将pH值调节至5。\n---\n\n# **合成工艺**\n\n1. **预浓缩溶液**：\n\n- 按照指示蒸发磷酸盐缓冲液、NaNO₂和乙酸钠。\n\n2. **反应设置（每个循环）**：\n\n- **10x反应缓冲液**：用水稀释1:10。加入**0.1%吐温-20**（每10毫升100µL 1%原液）。\n\n- 添加**TdT酶**:0.5 mg/mL最终浓度（从5 mg/mL储备中按1:10稀释）。\n\n- 添加**ntp**:0.25 mM最终浓度（从10 mM储备中按1:40稀释）。\n\n- 在30°C下孵育20分钟。\n\n3. **清洗**：使用含吐温-20的清洗缓冲液去除杂质。\n\n4. **脱保护**：用脱保护缓冲液（20分钟，pH 5-5.5）处理。\n\n5. **重复**：对每种碱进行循环反应、洗涤和脱保护。\n# 以上是根据论文总结的DNA合成工艺。", 
    }
]

export const workflow_step = "子任务：生成合成工艺";