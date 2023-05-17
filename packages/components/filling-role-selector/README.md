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

| Props          |  Type   | Required | Values | Default | Description                             |
| -------------- | :-----: | :------: | :----: | :-----: | --------------------------------------- |
| `roles`        | `array` |    -     |   -    |    -    | Array of roles                          |
| `users`        | `array` |    -     |   -    |    -    | Array of assigned users per role        |
| `onAddUser`    | `func`  |    -     |   -    |    -    | The function of adding a user to a role |
| `onRemoveUser` | `func`  |    -     |   -    |    -    | Function to remove a user from a role   |
