# FillingRoleSelector

### Usage

```js
import FillingRoleSelector from "@docspace/components/filling-role-selector";
```

```jsx
<FillingRoleSelector
  style={{ width: "480px", padding: "16px" }}
></FillingRoleSelector>
```

### Properties

| Props                 |      Type      | Required | Values | Default | Description                             |
| --------------------- | :------------: | :------: | :----: | :-----: | --------------------------------------- |
| `className`           |    `string`    |    -     |   -    |    -    | Accepts class                           |
| `descriptionEveryone` |    `string`    |    -     |   -    |    -    | Role description text Everyone          |
| `descriptionTooltip`  |    `string`    |    -     |   -    |    -    | Tooltip text                            |
| `id`                  |    `string`    |    -     |   -    |    -    | Accepts id                              |
| `onAddUser`           |     `func`     |    -     |   -    |    -    | The function of adding a user to a role |
| `onRemoveUser`        |     `func`     |    -     |   -    |    -    | Function to remove a user from a role   |
| `roles`               |    `array`     |    -     |   -    |    -    | Array of roles                          |
| `style`               | `obj`, `array` |    -     |   -    |    -    | Accepts css style                       |
| `users`               |    `array`     |    -     |   -    |    -    | Array of assigned users per role        |
