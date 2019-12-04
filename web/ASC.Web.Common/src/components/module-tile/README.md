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

| Props         |      Type      | Required | Values | Default | Description                             |
| ------------- | :------------: | :------: | :----: | :-----: | --------------------------------------- |
| `className`   |    `string`    |    -     |   -    |    -    | Accepts class                           |
| `description` |    `string`    |    -     |   -    |    -    | Description of primary tile             |
| `id`          |    `string`    |    -     |   -    |    -    | Accepts id                              |
| `imageUrl`    |    `string`    |    -     |   -    |    -    | Image url/path                          |
| `isPrimary`   |     `bool`     |    -     |   -    |    -    | Tells when the tile should be primary   |
| `link`        |    `string`    |    -     |   -    |    -    | Link to return on onClick               |
| `onClick`     |     `func`     |    ✅    |   -    |    -    | What the tile will trigger when clicked |
| `style`       | `obj`, `array` |    -     |   -    |    -    | Accepts css style                       |
| `title`       |    `string`    |    -     |   -    |    -    | Title of tile                           |
