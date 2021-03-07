import React from "react";

import CheckboxComponent from "./";

export default {
  title: "Components/Checkbox",
  component: CheckboxComponent,
  parameters: {
    docs: {
      description: { component: "Custom checkbox input" },
    },
  },
  argTypes: {
    onChange: {
      action: "onChange",
    },
  },
};

class Checkbox extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      isChecked: false,
    };
  }

  onChange = (e) => {
    this.props.onChange(e);
    this.setState({ isChecked: !this.state.isChecked });
  };

  render() {
    return (
      <CheckboxComponent
        {...this.props}
        isChecked={this.props.isChecked || this.state.isChecked}
        onChange={this.onChange}
      />
    );
  }
}
const Template = (args) => {
  return <Checkbox {...args} />;
};

const AllCheckboxesTemplate = (args) => {
  return (
    <div
      style={{
        display: "grid",
        gridTemplateColumns: "repeat( auto-fill, minmax(120px, 1fr) )",
        gridGap: "16px",
        alignItems: "center",
      }}
    >
      <Checkbox />
      <Checkbox isChecked={true} />
      <Checkbox isDisabled={true} />
      <Checkbox isIndeterminate={true} />
      <Checkbox label="Some label" />
    </div>
  );
};

export const Default = Template.bind({});
Default.args = {
  label: "Checkbox label",
};
export const AllCheckboxStates = AllCheckboxesTemplate.bind({});
