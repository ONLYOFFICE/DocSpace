# Buttons: HelpButton

## Usage

```js
import { HelpButton } from "asc-web-components";
```

#### Description

HelpButton is used for a action on a page.

#### Usage base

```js
<HelpButton tooltipContent={<Text.Body>Tooltip content</Text.Body>} />
```

#### Usage with link

```js
<HelpButton
  tooltipContent={
    <Text.Body>
      Tooltip content with{" "}
      <Link isHovered={true} href="/">
        link
      </Link>
    </Text.Body>
  }
/>
```

#### Usage

```js
<HelpButton
  tooltipContent={
    <>
      <p>This is a global react component tooltip</p>
      <p>You can put every thing here</p>
      <ul>
        <li>Word</li>
        <li>Chart</li>
        <li>Else</li>
      </ul>
    </>
  }
/>
```

#### Properties

| Props            | Type               | Required | Values                     | Default | Description       |
| ---------------- | ------------------ | :------: | -------------------------- | ------- | ----------------- |
| `tooltipContent` | `object or string` |    ✅    | -                          | -       | Tooltip content   |
| `place`          | `string`           |    -     | `top, right, bottom, left` | `top`   | Tooltip placement |
