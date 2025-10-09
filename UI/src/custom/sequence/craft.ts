export const craft_chat = [
    {
        role: "user",
        content: "生成测序工艺。",
    },
    {
        role: "assistant",
        content: "# 我搜索到了以下文献并生成如下工艺：\n1. 《Recent advances of DNA sequencing via nanopore-based technologies》\n2. 《Nanopore sequencing technology, bioinformatics and applications》\n3. 《CMOS Nanopore-based DNA Sequencing Systems: Recent Advancements and Future Prospects - A Review》\n\n### 1. 样本准备\n- **DNA提取**：从样本中提取DNA。\n- **质量检测**：使用凝胶电泳或分光光度计检测DNA质量和浓度。\n\n### 2. 库构建\n- **片段化**：将DNA切割成适合测序的片段。\n- **接头连接**：在DNA片段两端连接测序接头。\n- **扩增**：通过PCR扩增连接接头的DNA片段。\n\n### 3. 测序\n- **选择平台**：选择合适的测序平台。\n- **加载样本**：将DNA库加载到测序仪中。\n- **数据生成**：运行测序仪以生成序列数据。\n\n### 4. 数据分析\n- **数据质量控制**：使用软件评估测序数据的质量。\n- **比对与组装**：将序列比对到参考基因组或进行去新组装。\n- **变异检测**：识别基因组中的变异（如SNPs、Indels）。\n\n### 5. 结果解读\n- **生物信息学分析**：使用工具和数据库进行功能注释和生物学解读。\n- **报告生成**：根据分析结果生成报告，提供给研究人员或临床医生。\n\n### 注意事项\n- **实验设计**：根据研究目的选择合适的测序深度和覆盖度。\n- **质量控制**：每个步骤都需严格把控，以确保数据准确性。\n\n根据以上工艺即可完成测序流程。",
    }
]

export const craft_step = "子任务：生成测序工艺";