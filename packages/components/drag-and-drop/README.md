# DragAndDrop

Drag And Drop component can be used as Dropzone

#### See documentation: https://github.com/react-dropzone/react-dropzone

```js
import DragAndDrop from "@docspace/components/drag-and-drop";
```

```jsx
<DragAndDrop onDrop={onDrop} style={width: 200, height: 200, border: "5px solid #999"}>
  <Text style={textStyles} color="#999" fontSize="20px">
    Drop items here
  </Text>
</DragAndDrop>
```

### Properties

| Props        |    Type    | Required | Values | Default | Description                                                   |
| ------------ | :--------: | :------: | :----: | :-----: | ------------------------------------------------------------- |
| `dragging`   |   `bool`   |          |        | `false` | Show that the item is being dragged now.                      |
| `isDropZone` |   `bool`   |          |        | `false` | Sets the component as a dropzone                              |
| `onDrop`     | `function` |          |        |         | Occurs when the dragged element is dropped on the drop target |
