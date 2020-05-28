# DragAndDrop

Drag And Drop component can be used as Dropzone

### Usage

```js
import { DragAndDrop } from "asc-web-common";
```

```jsx
<DragAndDrop onDrop={onDrop} style={width: 200, height: 200, border: "5px solid #999"}>
  <Text style={textStyles} color="#999" fontSize="20px">
    Drop items here
  </Text>
</DragAndDrop>
```

### Properties

| Props         |    Type    | Required | Values | Default | Description                                                   |
| ------------- | :--------: | :------: | :----: | :-----: | ------------------------------------------------------------- |
| `draggable`   |   `bool`   |          |        | `false` | Sets the value of the draggable attribute to true             |
| `onDragStart` | `function` |          |        |         | Occurs when the user starts to drag an element                |
| `onDragEnter` | `function` |          |        |         | Occurs when the dragged element enters the drop target        |
| `onDragLeave` | `function` |          |        |         | Occurs when the dragged element leaves the drop target        |
| `onDragOver`  | `function` |          |        |         | Occurs when the dragged element is over the drop target       |
| `onDrop`      | `function` |          |        |         | Occurs when the dragged element is dropped on the drop target |
