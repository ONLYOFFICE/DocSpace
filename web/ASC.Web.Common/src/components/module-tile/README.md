# ModuleTile

Module tile is used for navigation to module page

### Usage

```js
import { ModuleTile } from "asc-web-common";
```

```jsx
<ModuleTile
  title="Documents"
  imageUrl="./modules/documents240.png"
  link="/products/files/"
  description="Create, edit and share documents. Collaborate on them in real-time. 100% compatibility with MS Office formats guaranteed."
  isPrimary={true}
  onClick={action("onClick")}
/>
```

### Properties

| Props         |   Type   | Required | Values | Default | Description                             |
| ------------- | :------: | :------: | :----: | :-----: | --------------------------------------- |
| `title`       | `string` |    ✅    |   -    |    -    | Title of tile                           |
| `imageUrl`    | `string` |    ✅    |   -    |    -    | Image url/path                          |
| `link`        | `string` |    ✅    |   -    |    -    | Link to return on onClick               |
| `description` | `string` |    -     |   -    |    -    | Description of primary tile             |
| `isPrimary`   |  `bool`  |    -     |   -    |    -    | Tells when the tile should be primary   |
| `onClick`     |  `func`  |    ✅    |   -    |    -    | What the tile will trigger when clicked |
