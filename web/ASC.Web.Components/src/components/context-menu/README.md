# ContextMenu

## Usage

```js
import { ContextMenu } from "asc-web-components";
```

#### Description

ContextMenu is used for a call context actions on a page.

Implemented as part of RowContainer component.

#### Usage

For use within separate component it is necessary to determine active zone and events for calling and transferring options in menu.

In particular case, state is created containing options for particular Row element and passed to component when called.

```js
<ContextMenu targetAreaId="rowContainer" options={[]} />
```

#### Properties

| Props          | Type     | Required | Values | Default | Description              |
| -------------- | -------- | :------: | ------ | ------- | ------------------------ |
| `options`      | `array`  |    -     | -      | []      | DropDownItems collection |
| `targetAreaId` | `string` |    -     | -      | -       | Id of container apply to |
