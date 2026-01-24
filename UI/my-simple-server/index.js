const express = require('express');
const cors = require('cors');
const app = express();
const port = 3001;

app.use(cors());

app.get('/', (req, res) => {
   //res.json({ state: 'sequence', taskId : 3 });
   res.json({ state: 'synthesis', taskId : 5 });
});

app.listen(port, () => {
  console.log(`Server is running at http://localhost:${port}`);
});

