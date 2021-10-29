# ContextMenu

ContextMenu is used for a call context actions on a page.

> Implemented as part of RowContainer component.

### Usage

```js
import NewContextMenu from "@appserver/components/new-context-menu";
```

```jsx
<NewContextMenu model={defaultModel} />
```

For use within separate component it is necessary to determine active zone and events for calling and transferring options in menu.

In particular case, state is created containing options for particular Row element and passed to component when called.

ContextMenu contain MenuItem component and can take from the props model(all view)
and header(show only tablet or mobile, when view changed).

### Properties

| Props          |      Type      | Required | Values |    Default    | Description              |
| -------------- | :------------: | :------: | :----: | :-----------: | ------------------------ |
| `className`    |    `string`    |    -     |   -    |       -       | Accepts class            |
| `id`           |    `string`    |    -     |   -    | `contextMenu` | Accepts id               |
| `model`        |    `array`     |    -     |   -    |     `[ ]`     | Items collection         |
| `header`       |    `object`    |    -     |   -    |     `{}`      | ContextMenu header       |
| `style`        | `obj`, `array` |    -     |   -    |       -       | Accepts css style        |
| `targetAreaId` |    `string`    |    -     |   -    |       -       | Id of container apply to |
| `withBackdrop` |     `bool`     |    -     |   -    |    `true`     | Used to display backdrop |
