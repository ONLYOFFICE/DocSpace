# Room logo

Room logo allow you display default room logo depend on type and private

### Usage

```js
import RoomLogo from "@appserver/components/room-logo";
```

```jsx
<RoomLogo type={"custom"} isPrivacy={false} />
```

| Props       |                             Type                             | Required | Values | Default | Description              |
| ----------- | :----------------------------------------------------------: | :------: | :----: | :-----: | ------------------------ |
| `id`        |                           `string`                           |    -     |   -    |    -    | Accepts id               |
| `className` |                           `string`                           |    -     |   -    |    -    | Accepts class            |
| `style`     |                        `obj`, `array`                        |    -     |   -    |    -    | Accepts css style        |
| `type`      | `[ "view", "review", "fill", "editing","custom", "archive"]` |    -     |   -    |   ``    | Accepts the type of room |
| `isPrivacy` |                          `boolean`                           |    -     |   -    | `false` | Accepts the privacy room |
