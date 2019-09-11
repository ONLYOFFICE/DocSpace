# Avatar

## Usage

```js
import { Avatar } from "asc-web-components";
```

#### Description

Required to display user avatar on page.

If you want to create an avatar with initials, only *first letter of first two words* of line is used.

#### Usage

```js
<Avatar size="max" role="admin" source="" userName="" editing={false} />
```

#### Properties

| Props      | Type     | Required | Values                            | Default  | Description                                |
| ---------- | -------- | :------: | --------------------------------- | -------- | ------------------------------------------ |
| `size`     | `oneOf`  |    -     | `max`, `big`, `medium`, `small`   | `medium` | Tells what size avatar should be displayed |
| `role`     | `oneOf`  |    -     | `owner`, `admin`, `guest`, `user` | -        | Adds a user role table                     |
| `source`   | `string` |    -     | -                                 | -        | Avatar image source                        |
| `userName` | `string` |    -     | -                                 | -        | Need to create an avatar with initials     |
| `editing`  | `bool`   |    -     | -                                 | `false`  | Displays avatar edit layer                 |
