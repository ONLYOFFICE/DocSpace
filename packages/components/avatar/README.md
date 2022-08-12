# Avatar

Used to display an avatar or brand.

### Usage

```js
import Avatar from "@docspace/components/avatar";
```

```jsx
<Avatar size="max" role="admin" source="" userName="" editing={false} />
```

If you want to create an avatar with initials, only _first letter of first two words_ of line is used.

### Properties

| Props        |      Type      | Required |                 Values                 |   Default    | Description                                              |
| ------------ | :------------: | :------: | :------------------------------------: | :----------: | -------------------------------------------------------- |
| `size`       |    `oneOf`     |    -     | `max`, `big`, `medium`, `small`, `min` |   `medium`   | Size of avatar                                           |
| `role`       |    `oneOf`     |    -     |   `owner`, `admin`, `guest`, `user`    |    `user`    | Adds a user role table                                   |
| `source`     |    `string`    |    -     |                   -                    |      -       | The address of the image for an image avatar             |
| `isIcon`     |     `bool`     |    -     |                   -                    |   `false`    | Set `true` if `.svg` is provided as `source` prop        |
| `userName`   |    `string`    |    -     |                   -                    |      -       | Need to create an avatar with initials                   |
| `editing`    |     `bool`     |    -     |                   -                    |   `false`    | Displays avatar edit layer                               |
| `editLabel`  |    `string`    |    -     |                   -                    | `Edit photo` | Label for editing layer                                  |
| `editAction` |     `func`     |    -     |                   -                    |      -       | Function called when the avatar change button is pressed |
| `className`  |    `string`    |    -     |                   -                    |      -       | Accepts class                                            |
| `id`         |    `string`    |    -     |                   -                    |      -       | Accepts id                                               |
| `style`      | `obj`, `array` |    -     |                   -                    |      -       | Accepts css style                                        |
