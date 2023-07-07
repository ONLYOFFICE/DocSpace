import React from "react";

import Icon from "PUBLIC_DIR/images/button.alert.react.svg";
import Button from "./";

export default {
  title: "Components/Button",
  component: Button,
  parameters: {
    docs: {
      description: {
        component: "Button is used for a action on a page.",
      },
    },
    design: {
      type: "figma",
      url: "https://www.figma.com/file/ZiW5KSwb4t7Tj6Nz5TducC/UI-Kit-DocSpace-1.0.0?type=design&node-id=62-3582&mode=design&t=TBNCKMQKQMxr44IZ-0",
    },
  },
};

const sizes = ["extraSmall", "small", "normal", "medium"];

const Wrapper = (props) => (
  <div
    style={{
      display: "grid",
      gridTemplateColumns: props.isScale
        ? "1fr"
        : "repeat( auto-fill, minmax(180px, 1fr) )",
      gridGap: "16px",
      alignItems: "center",
    }}
  >
    {props.children}
  </div>
);

const Template = (args) => (
  <Button {...args} onClick={() => alert("Button clicked")} />
);
const PrimaryTemplate = (args) => {
  return (
    <Wrapper>
      {sizes.map((size) => (
        <Button
          key={`all-primary-${size}`}
          {...args}
          primary
          scale={false}
          size={size}
          label={`Primary ${size[0].toUpperCase()}${size.slice(1)}`}
        />
      ))}
    </Wrapper>
  );
};

const SecondaryTemplate = (args) => {
  return (
    <Wrapper>
      {sizes.map((size) => (
        <Button
          key={`all-secondary-${size}`}
          {...args}
          scale={false}
          size={size}
          label={`Secondary ${size[0].toUpperCase()}${size.slice(1)}`}
        />
      ))}
    </Wrapper>
  );
};

const WithIconTemplate = (args) => {
  return (
    <Wrapper>
      {sizes.map((size) => (
        <Button
          key={`all-icon-prim-${size}`}
          {...args}
          primary
          size={size}
          icon={<Icon />}
          label={`With Icon ${size[0].toUpperCase()}${size.slice(1)}`}
        />
      ))}
      {sizes.map((size) => (
        <Button
          key={`all-icon-sec-${size}`}
          {...args}
          size={size}
          icon={<Icon />}
          label={`With Icon ${size[0].toUpperCase()}${size.slice(1)}`}
        />
      ))}
    </Wrapper>
  );
};

const IsLoadingTemplate = (args) => {
  return (
    <Wrapper>
      {sizes.map((size) => (
        <Button
          key={`all-load-prim-${size}`}
          {...args}
          primary
          size={size}
          isLoading={true}
          label={`Loading ${size[0].toUpperCase()}${size.slice(1)}`}
        />
      ))}
      {sizes.map((size) => (
        <Button
          key={`all-load-sec-${size}`}
          {...args}
          size={size}
          isLoading={true}
          label={`Loading ${size[0].toUpperCase()}${size.slice(1)}`}
        />
      ))}
    </Wrapper>
  );
};

const ScaleTemplate = (args) => {
  return (
    <Wrapper isScale>
      {sizes.map((size) => (
        <Button
          key={`all-scale-prim-${size}`}
          {...args}
          primary
          size={size}
          label={`Scale ${size[0].toUpperCase()}${size.slice(1)}`}
        />
      ))}
      {sizes.map((size) => (
        <Button
          key={`all-scale-sec-${size}`}
          {...args}
          size={size}
          label={`Scale ${size[0].toUpperCase()}${size.slice(1)}`}
        />
      ))}
    </Wrapper>
  );
};

const DisabledTemplate = (args) => {
  return (
    <Wrapper>
      {sizes.map((size) => (
        <Button
          key={`all-disabled-prim-${size}`}
          {...args}
          primary
          size={size}
          isDisabled={true}
          label={`Disabled ${size[0].toUpperCase()}${size.slice(1)}`}
        />
      ))}
      {sizes.map((size) => (
        <Button
          key={`all-disabled-sec-${size}`}
          {...args}
          size={size}
          isDisabled={true}
          label={`Disabled ${size[0].toUpperCase()}${size.slice(1)}`}
        />
      ))}
    </Wrapper>
  );
};

const ClickedTemplate = (args) => {
  return (
    <Wrapper>
      {sizes.map((size) => (
        <Button
          key={`all-clicked-prim-${size}`}
          {...args}
          primary
          size={size}
          isClicked={true}
          label={`Clicked ${size[0].toUpperCase()}${size.slice(1)}`}
        />
      ))}
      {sizes.map((size) => (
        <Button
          key={`all-clicked-sec-${size}`}
          {...args}
          size={size}
          isClicked={true}
          label={`Clicked ${size[0].toUpperCase()}${size.slice(1)}`}
        />
      ))}
    </Wrapper>
  );
};

const HoveredTemplate = (args) => {
  return (
    <Wrapper>
      {sizes.map((size) => (
        <Button
          key={`all-hovered-prim-${size}`}
          {...args}
          primary
          size={size}
          isHovered={true}
          label={`Hovered ${size[0].toUpperCase()}${size.slice(1)}`}
        />
      ))}
      {sizes.map((size) => (
        <Button
          key={`all-hovered-sec-${size}`}
          {...args}
          size={size}
          isHovered={true}
          label={`Hovered ${size[0].toUpperCase()}${size.slice(1)}`}
        />
      ))}
    </Wrapper>
  );
};

export const Default = Template.bind({});
Default.args = {
  size: "extraSmall",
  label: "Button",
};
export const PrimaryButtons = PrimaryTemplate.bind({});
export const SecondaryButtons = SecondaryTemplate.bind({});
export const WithIconButtons = WithIconTemplate.bind({});
export const IsLoadingButtons = IsLoadingTemplate.bind({});
export const ScaleButtons = ScaleTemplate.bind({});
export const DisabledButtons = DisabledTemplate.bind({});
export const ClickedButtons = ClickedTemplate.bind({});
export const HoveredButtons = HoveredTemplate.bind({});
HoveredButtons.argTypes = {
  isHovered: { table: { disable: true } },
};
