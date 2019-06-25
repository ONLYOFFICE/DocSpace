# ContextMenuButton

#### Description

ContextMenuButton is used for displaying context menu actions on a list's item

#### Usage

```js
import { ContextMenuButton } from 'asc-web-components';

<ContextMenuButton
    title="Actions"
    getData={() => [{key: 'key', label: 'label', onClick: () => alert('label')}]}
/>
```

#### Properties

| Props   | Type     | Required | Values                                            | Default  | Description              |
| ------- | -------- | :------: | ------------------------------------------------- | -------  | ------------------------ |
| `title` | `string` |          |                                                   | -        | Specifies the icon title |
| `size`  | `string` |          | `small`, `medium`, `big` ,`scale`                 | `medium` | Specifies the icon size  |
| `color` | `string` |          |                                                   | -        | Specifies the icon color |