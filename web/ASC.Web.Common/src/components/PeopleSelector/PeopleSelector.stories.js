/* eslint-disable react/prop-types */
import React from "react";
import { storiesOf } from "@storybook/react";
import { withKnobs, boolean } from "@storybook/addon-knobs/react";
import Section from "../../../.storybook/decorators/section";

import PeopleSelector from ".";
import { Button } from "asc-web-components";
import withProvider from "../../../.storybook/decorators/redux";
import { text } from "@storybook/addon-knobs";
//import withReadme from "storybook-readme/with-readme";
//import Readme from "./README.md";

class PeopleSelectorExample extends React.Component {
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
        <PeopleSelector
          isOpen={this.state.isOpen}
          useFake={true}
          isMultiSelect={boolean("isMultiSelect", true)}
          onSelect={(data) => {
            console.log("onSelect", data);
            this.toggle();
          }}
          onCancel={this.onCancel}
          defaultOption={{
            id: "777",
            groups: [],
            displayName: "Boris Johnson",
            avatar: "",
            title: "Prime Minister of the United Kingdom",
            email: "boris.johnson@example.com",
          }}
          defaultOptionLabel={text("defaultOptionLabel", "Me")}
        />
      </div>
    );
  }
}

storiesOf("Components|PeopleSelector", module)
  .addDecorator(withProvider)
  .addDecorator(withKnobs)
  //.addDecorator(withReadme(Readme))
  .addParameters({ options: { addonPanelInRight: false } })
  .add("base", () => {
    return (
      <Section>
        <PeopleSelectorExample />
      </Section>
    );
  });
