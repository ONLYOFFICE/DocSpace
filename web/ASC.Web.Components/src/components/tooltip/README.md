# Tooltip

Custom tooltip

#### See documentation: https://github.com/wwayne/react-tooltip

### Usage with IconButton

```js
import { Tooltip, IconButton, Text } from "asc-web-components";
```

```jsx
  <div
    style={BodyStyle}
    data-for="tooltipContent"
    data-tip="You tooltip content"
    data-event="click focus"
    data-offset="{'top': 100, 'right': 100}"
    data-place="top"
  >
    <IconButton isClickable={true} size={20} iconName="QuestionIcon" />
  </div>
  <Tooltip
    id="tooltipContent"
    getContent={dataTip => <Text.Body fontSize={13}>{dataTip}</Text.Body>}
    effect="float"
    place="top"
    maxWidth={320}
  />
```

### Usage with array

```js
import { Tooltip, Link, Text } from "asc-web-components";
```

```js
const arrayUsers = [
  { key: "user_1", name: "Bob", email: "Bob@gmail.com", position: "developer" },
  {
    key: "user_2",
    name: "John",
    email: "John@gmail.com",
    position: "developer"
  },
  {
    key: "user_3",
    name: "Kevin",
    email: "Kevin@gmail.com",
    position: "developer"
  },
  {
    key: "user_4",
    name: "Alex",
    email: "Alex@gmail.com",
    position: "developer"
  },
  {
    key: "user_5",
    name: "Tomas",
    email: "Tomas@gmail.com",
    position: "developer"
  }
];
```

```jsx
  <h5 style={{ marginLeft: -5 }}>Hover group</h5>
  <Link data-for="group" data-tip={0}>Bob</Link><br />
  <Link data-for="group" data-tip={1}>John</Link><br />
  <Link data-for="group" data-tip={2}>Kevin</Link><br />
  <Link data-for="group" data-tip={3}>Alex</Link><br />
  <Link data-for="group" data-tip={4}>Tomas</Link>
```

```jsx
<Tooltip
  id="group"
  offsetRight={90}
  getContent={dataTip =>
    dataTip ? (
      <div>
        <Text.Body isBold={true} fontSize={16}>
          {arrayUsers[dataTip].name}
        </Text.Body>
        <Text.Body color="#A3A9AE" fontSize={13}>
          {arrayUsers[dataTip].email}
        </Text.Body>
        <Text.Body fontSize={13}>{arrayUsers[dataTip].position}</Text.Body>
      </div>
    ) : null
  }
/>
```

### YouComponent Properties

| Props         |   Type   | Required |              Values              | Default | Description                       |
| ------------- | :------: | :------: | :------------------------------: | :-----: | --------------------------------- |
| `data-tip`    | `string` |    -     |                -                 |    -    | Required if you need to component |
| `data-event`  | `string` |    -     |          `click, focus`          |    -    | Custom event to trigger tooltip   |
| `data-offset` | `string` |    -     | `top`, `left`, `right`, `bottom` |    -    | Offset of current tooltip         |
| `data-place`  | `string` |    -     | `top`, `right`, `bottom`, `left` |    -    | Tooltip placement                 |
| `data-for`    | `string` |    ✅    |                -                 |    -    | Corresponds to the id of Tooltip  |

### ReactTooltip Properties

| Props          |   Type   | Required |              Values              | Default | Description                          |
| -------------- | :------: | :------: | :------------------------------: | :-----: | ------------------------------------ |
| `id`           | `string` |    ✅    |                -                 |    -    | Used as HTML id property             |
| `getContent`   |  `func`  |    -     |                                  |    -    | Generate the tip content dynamically |
| `effect`       | `string` |    -     |         `float`, `solid`         | `float` | Behavior of tooltip                  |
| `place`        | `string` |    -     | `top`, `right`, `bottom`, `left` |  `top`  | Global tooltip placement             |
| `offsetTop`    | `number` |    -     |                -                 |    -    | Offset top all tooltips on page      |
| `offsetRight`  | `number` |    -     |                -                 |    -    | Offset right all tooltips on page    |
| `offsetBottom` | `number` |    -     |                -                 |    -    | Offset bottom all tooltips on page   |
| `offsetLeft`   | `number` |    -     |                -                 |    -    | Offset left all tooltips on page     |
| `maxWidth`     | `number` |    -     |                -                 |  `340`  | Set max width of tooltip             |
