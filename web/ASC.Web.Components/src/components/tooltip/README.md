# Tooltip

#### Description

Custom tooltip

#### Usage

```js
import { Tooltip } from "asc-web-components";

  <a data-for="tooltipContent" data-tip="Tooltip content" data-event="click focus">
    (❂‿❂) You component
  </a>

  <Tooltip
    id="tooltipContent"
    getContent={dataTip => <Text.Body fontSize={13}>{dataTip}</Text.Body>}
    type="light"
    effect="float"
    place="top"
  />;
```

#### YouComponent Properties

| Props        | Type     | Required | Values | Default | Description                       |
| ------------ | -------- | :------: | ------ | ------- | --------------------------------- |
| `data-tip`   | `string` |    ✅    | -      | -       | Required if you need to component |
| `data-for`   | `string` |    ✅    | -      | -       | Corresponds to the id of Tooltip  |
| `data-event` | `string` |    -     | -      | -       | Custom event to trigger tooltip   |

#### ReactTooltip Properties

| Props        | Type     | Required | Values                                 | Default | Description                          |
| ------------ | -------- | :------: | -------------------------------------- | ------- | ------------------------------------ |
| `id`         | `string` |    ✅    | -                                      | -       | Used as HTML id property             |
| `getContent` | `func`   |    -     |                                        |         | Generate the tip content dynamically |
| `type`       | `string` |    -     | `success, warning, error, info, light` | `light` | Tooltip theme                        |
| `effect`     | `string` |    -     | `float, solid`                         | `float` | Behaviour of tooltip                 |
