# Tooltip

Custom tooltip

#### See documentation: https://github.com/wwayne/react-tooltip

### Usage with IconButton

```js
import Tooltip from "@docspace/components/tooltip";
import IconButton from "@docspace/components/icon-button";
import Text from "@docspace/components/text";
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
    <IconButton isClickable={true} size={20} iconName="static/images/question.react.svg" />
  </div>
  <Tooltip
    id="tooltipContent"
    getContent={dataTip => <Text fontSize='13px'>{dataTip}</Text>}
    effect="float"
    place="top"
    maxWidth={320}
  />
```

### Usage with array

```js
import { Tooltip, Link, Text } from "@docspace/components";
```

```js
const arrayUsers = [
  { key: "user_1", name: "Bob", email: "Bob@gmail.com", position: "developer" },
  {
    key: "user_2",
    name: "John",
    email: "John@gmail.com",
    position: "developer",
  },
  {
    key: "user_3",
    name: "Kevin",
    email: "Kevin@gmail.com",
    position: "developer",
  },
  {
    key: "user_4",
    name: "Alex",
    email: "Alex@gmail.com",
    position: "developer",
  },
  {
    key: "user_5",
    name: "Tomas",
    email: "Tomas@gmail.com",
    position: "developer",
  },
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
  getContent={(dataTip) =>
    dataTip ? (
      <div>
        <Text isBold={true} fontSize="16px">
          {arrayUsers[dataTip].name}
        </Text>
        <Text color="#A3A9AE" fontSize="13px">
          {arrayUsers[dataTip].email}
        </Text>
        <Text fontSize="13px">{arrayUsers[dataTip].position}</Text>
      </div>
    ) : null
  }
/>
```

### YouComponent Properties

| Props         |   Type   | Required |              Values              | Default | Description                       |
| ------------- | :------: | :------: | :------------------------------: | :-----: | --------------------------------- |
| `data-event`  | `string` |    -     |          `click, focus`          |    -    | Custom event to trigger tooltip   |
| `data-for`    | `string` |    ✅    |                -                 |    -    | Corresponds to the id of Tooltip  |
| `data-offset` | `string` |    -     | `top`, `left`, `right`, `bottom` |    -    | Offset of current tooltip         |
| `data-place`  | `string` |    -     | `top`, `right`, `bottom`, `left` |    -    | Tooltip placement                 |
| `data-tip`    | `string` |    -     |                -                 |    -    | Required if you need to component |

### ReactTooltip Properties

| Props          |      Type      | Required |              Values              | Default | Description                          |
| -------------- | :------------: | :------: | :------------------------------: | :-----: | ------------------------------------ |
| `className`    |    `string`    |    -     |                -                 |    -    | Accepts class                        |
| `effect`       |    `string`    |    -     |         `float`, `solid`         | `float` | Behavior of tooltip                  |
| `getContent`   |     `func`     |    -     |                                  |    -    | Generate the tip content dynamically |
| `id`           |    `string`    |    ✅    |                -                 |    -    | Used as HTML id property             |
| `maxWidth`     |    `number`    |    -     |                -                 |  `340`  | Set max width of tooltip             |
| `offsetBottom` |    `number`    |    -     |                -                 |    -    | Offset bottom all tooltips on page   |
| `offsetLeft`   |    `number`    |    -     |                -                 |    -    | Offset left all tooltips on page     |
| `offsetRight`  |    `number`    |    -     |                -                 |    -    | Offset right all tooltips on page    |
| `offsetTop`    |    `number`    |    -     |                -                 |    -    | Offset top all tooltips on page      |
| `place`        |    `string`    |    -     | `top`, `right`, `bottom`, `left` |  `top`  | Global tooltip placement             |
| `style`        | `obj`, `array` |    -     |                -                 |    -    | Accepts css style                    |
