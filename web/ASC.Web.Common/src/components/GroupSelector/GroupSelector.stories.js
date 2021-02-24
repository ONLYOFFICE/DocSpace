/* eslint-disable react/prop-types */
import React from "react";
import { storiesOf } from "@storybook/react";
import { withKnobs, boolean } from "@storybook/addon-knobs/react";
import Section from "../../../.storybook/decorators/section";
import GroupSelector from ".";
import { Button } from "asc-web-components";
//import withReadme from "storybook-readme/with-readme";
//import Readme from "./README.md";

class GroupSelectorExample extends React.Component {
  constructor(props) {
    super(props);

    this.buttonRef = React.createRef();

    this.state = {
      isOpen: false,
    };
  }

  toggle = () => {
    this.setState({
      isOpen: !this.state.isOpen,
    });
  };

  onCancel = (e) => {
    if (this.buttonRef.current.contains(e.target)) {
      console.log("onCancel skipped");
      return;
    }

    console.log("onCancel");
    this.toggle();
  };

  render() {
    return (
      <div style={{ position: "relative" }}>
        <Button
          label="Toggle dropdown"
          onClick={this.toggle}
          ref={this.buttonRef}
        />
        <GroupSelector
          isOpen={this.state.isOpen}
          useFake={true}
          isMultiSelect={boolean("isMultiSelect", true)}
          onSelect={(data) => {
            console.log("onSelect", data);
            this.toggle();
          }}
          onCancel={this.onCancel}
        />
      </div>
    );
  }
}

storiesOf("Components|GroupSelector", module)
  .addDecorator(withKnobs)
  //.addDecorator(withReadme(Readme))
  .addParameters({ options: { addonPanelInRight: false } })
  .add("base", () => {
    return (
      <Section>
        <GroupSelectorExample />
      </Section>
    );
  });
