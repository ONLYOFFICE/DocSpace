# GroupButtonsMenu

## Usage

```js
import { GroupButtonsMenu } from 'asc-web-components';
```

#### Description

Menu for group actions on a page.

#### Usage

```js
<GroupButtonsMenu checked={false} menuItems={menuItems} visible={true} />
```

#### Properties

| Props              | Type     | Required | Values                      | Default   | Description                                               |
| ------------------ | -------- | :------: | --------------------------- | --------- | --------------------------------------------------------- |
| `checked`          | `bool`   | -        | -                           | `false`   | Sets initial value of checkbox                            |
| `menuItems`        | `object` | -        | -                           | -         | Button collection                                         |
| `visible`          | `bool`   | -        | -                           | -         | Sets menu visibility                                      |
| `moreLabel`        | `string` | -        | -                           | `More`    | Label for more button                                     |
| `closeTitle`       | `string` | -        | -                           | `Close`   | Title for close menu button                               |
