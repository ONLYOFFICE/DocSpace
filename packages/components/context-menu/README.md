# ContextMenu

ContextMenu is used for a call context actions on a page.

> Implemented as part of RowContainer component.

### Usage

```js
import ContextMenu from "@docspace/components/context-menu";
```

```jsx
<ContextMenu targetAreaId="rowContainer" options={[]} />
```

For use within separate component it is necessary to determine active zone and events for calling and transferring options in menu.

In particular case, state is created containing options for particular Row element and passed to component when called.

### Properties

| Props          |      Type      | Required | Values |    Default    | Description              |
| -------------- | :------------: | :------: | :----: | :-----------: | ------------------------ |
| `className`    |    `string`    |    -     |   -    |       -       | Accepts class            |
| `id`           |    `string`    |    -     |   -    | `contextMenu` | Accepts id               |
| `options`      |    `array`     |    -     |   -    |     `[ ]`     | DropDownItems collection |
| `style`        | `obj`, `array` |    -     |   -    |       -       | Accepts css style        |
| `targetAreaId` |    `string`    |    -     |   -    |       -       | Id of container apply to |
| `withBackdrop` |     `bool`     |    -     |   -    |    `true`     | Used to display backdrop |
