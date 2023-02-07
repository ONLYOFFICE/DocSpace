# Room logo

Room logo allow you display default room logo depend on type and private

### Usage

```js
import RoomLogo from "@docspace/components/room-logo";
```

```jsx
<RoomLogo type={"custom"} isPrivacy={false} isArchive={false} />
```

| Props             |      Type      | Required | Values | Default | Description                                          |
| ----------------- | :------------: | :------: | :----: | :-----: | ---------------------------------------------------- |
| `id`              |    `string`    |    -     |   -    |    -    | Accepts id                                           |
| `className`       |    `string`    |    -     |   -    |    -    | Accepts class                                        |
| `style`           | `obj`, `array` |    -     |   -    |    -    | Accepts css style                                    |
| `type`            |    `number`    |    -     |   -    |   ``    | Accepts the type of room                             |
| `isPrivacy`       |   `boolean`    |    -     |   -    | `false` | Accepts the privacy room                             |
| `isArchive`       |   `boolean`    |    -     |   -    | `false` | Accepts the archive room                             |
| `withCheckbox`    |   `boolean`    |    -     |   -    | `false` | Accepts checkbox when row/tile is hovered or checked |
| `isChecked`       |   `boolean`    |    -     |   -    | `false` | Accepts the checked state to checkbox                |
| `isIndeterminate` |   `boolean`    |    -     |   -    | `false` | Accepts the indeterminate state to checkbox          |
| `onChange`        |   `function`   |    -     |   -    |    -    | Accepts the onChange checkbox callback function      |
