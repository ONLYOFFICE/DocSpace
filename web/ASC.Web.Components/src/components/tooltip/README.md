# Tooltip

#### Description

Custom tooltip

#### Usage with IconButton

```js
import { Tooltip, IconButton, Text } from "asc-web-components";

  <div
    style={BodyStyle}
    data-for="tooltipContent"
    data-tip="You tooltip content"
    data-event="click focus"
  >
    <IconButton isClickable={true} size={20} iconName="QuestionIcon" />
  </div>
  <Tooltip
    id="tooltipContent"
    getContent={dataTip => <Text.Body fontSize={13}>{dataTip}</Text.Body>}
    effect={select("effect", arrayEffects, "float")}
    place={select("place", arrayPlaces, "top")}
    maxWidth={number("maxWidth", 320)}
  />

```

#### Usage with array

```js
import { Tooltip, Link, Text } from "asc-web-components";

const arrayUsers = [
  { key: "user_1", name: "Bob", email: "Bob@gmail.com", position: "developer" },
  { key: "user_2", name: "John", email: "John@gmail.com", position: "developer" },
  { key: "user_3", name: "Kevin", email: "Kevin@gmail.com", position: "developer" },
  { key: "user_4", name: "Alex", email: "Alex@gmail.com", position: "developer" },
  { key: "user_5", name: "Tomas", email: "Tomas@gmail.com", position: "developer" }
];

  <h5 style={{ marginLeft: -5 }}>Hover group</h5>
  <Link data-for="group" data-tip={0}>Bob</Link><br />
  <Link data-for="group" data-tip={1}>John</Link><br />
  <Link data-for="group" data-tip={2}>Kevin</Link><br />
  <Link data-for="group" data-tip={3}>Alex</Link><br />
  <Link data-for="group" data-tip={4}>Tomas</Link>

  <Tooltip
    id="group"
    offset={{ right: 90 }}
    getContent={dataTip =>
      dataTip ? (
        <div>
          <Text.Body isBold={true} fontSize={16}>{arrayUsers[dataTip].name}</Text.Body>
          <Text.Body color="#A3A9AE" fontSize={13}>{arrayUsers[dataTip].email}</Text.Body>
          <Text.Body fontSize={13}>{arrayUsers[dataTip].position}</Text.Body>
        </div>
      ) : null
    }

```

#### YouComponent Properties

| Props        | Type     | Required | Values | Default | Description                       |
| ------------ | -------- | :------: | ------ | ------- | --------------------------------- |
| `data-tip`   | `string` |    -     | -      | -       | Required if you need to component |
| `data-event` | `string` |    -     | -      | -       | Custom event to trigger tooltip   |
| `data-for`   | `string` |    ✅    | -      | -       | Corresponds to the id of Tooltip  |

#### ReactTooltip Properties

| Props        | Type     | Required | Values                                 | Default | Description                          |
| ------------ | -------- | :------: | -------------------------------------- | ------- | ------------------------------------ |
| `id`         | `string` |    ✅    | -                                      | -       | Used as HTML id property             |
| `getContent` | `func`   |    -     |                                        | -       | Generate the tip content dynamically |
| `effect`     | `string` |    -     | `float, solid`                         | `float` | Behaviour of tooltip                 |
| `offset`     | `object` |    -     | `top, left, right, bottom`             | -       | Offset of tooltip                    |
| `maxWidth`   | `number` |    -     | -                                      | `340`   | Set max width of tooltip             |
