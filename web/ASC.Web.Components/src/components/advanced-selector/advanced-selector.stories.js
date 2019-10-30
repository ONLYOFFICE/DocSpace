/* eslint-disable react/prop-types */
import React from "react";
import { storiesOf } from "@storybook/react";
import { action } from "@storybook/addon-actions";
import {
  withKnobs,
  text,
  number,
  boolean,
  select
} from "@storybook/addon-knobs/react";
import withReadme from "storybook-readme/with-readme";
import Readme from "./README.md";
import AdvancedSelector from "./";
import Section from "../../../.storybook/decorators/section";
import Button from "../button";
import { isEqual, slice } from "lodash";
import { name } from "faker";

function getRandomInt(min, max) {
  return Math.floor(Math.random() * (max - min)) + min;
}
const sizes = ["compact", "full"];
const displayTypes = ["dropdown", "aside"];

class ADSelectorExample extends React.Component {
  constructor(props) {
    super(props);

    const { isOpen, total } = props;

    const groups = this.generateGroups();
    const users = this.generateUsers(total, groups);

    this.state = this.getDefaultState(isOpen, groups, users);
  }

  getDefaultState = (isOpen, groups, allOptions) => {
    return {
      isOpen: isOpen,
      allOptions,
      options: [],
      groups,
      hasNextPage: true,
      isNextPageLoading: false
    };
  };

  generateGroups = () => {
    return [
      {
        key: "group-all",
        label: "All groups",
        total: 0
      },
      {
        key: "group-dev",
        label: "Development",
        total: 0
      },
      {
        key: "group-management",
        label: "Management",
        total: 0
      },
      {
        key: "group-marketing",
        label: "Marketing",
        total: 0
      },
      {
        key: "group-mobile",
        label: "Mobile",
        total: 0
      },
      {
        key: "group-support",
        label: "Support",
        total: 0
      },
      {
        key: "group-web",
        label: "Web",
        total: 0
      }
    ];
  };

  generateUsers = (count, groups) => {
    return Array.from({ length: count }, (v, index) => {
      const additional_group = groups[getRandomInt(1, 6)];
      groups[0].total++;
      additional_group.total++;
      return {
        key: `user${index}`,
        groups: ["group-all", additional_group.key],
        label: `${name.findName()} (User${index})`
      };
    });
  };

  loadNextPage = (startIndex, stopIndex) => {
    console.log(
      `loadNextPage(startIndex=${startIndex}, stopIndex=${stopIndex})`
    );
    this.setState({ isNextPageLoading: true }, () => {
      setTimeout(() => {
        const { options } = this.state;
        const newOptions = [...options].concat(
          slice(this.state.allOptions, startIndex, startIndex + 100)
        );

        this.setState({
          hasNextPage: newOptions.length < this.props.total,
          isNextPageLoading: false,
          options: newOptions
        });
      }, 500);
    });
  };

  componentDidUpdate(prevProps) {
    const { total, options, isOpen } = this.props;
    if (!isEqual(prevProps.options, options)) {
      this.setState({
        options: options
      });
    }

    if (isOpen !== prevProps.isOpen) {
      this.setState({
        isOpen: isOpen
      });
    }

    if (total !== prevProps.total) {
      const groups = this.generateGroups();
      const users = this.generateUsers(total, groups);
      this.setState(this.getDefaultState(isOpen, groups, users));
    }
  }

  toggle = () => {
    this.setState({
      isOpen: !this.state.isOpen
    });
  };

  render() {
    const {
      isOpen,
      options,
      groups,
      selectedOptions,
      selectedGroups,
      hasNextPage,
      isNextPageLoading
    } = this.state;
    return (
      <div style={{position: 'relative'}}>
        <Button label="Toggle dropdown" onClick={this.toggle} />
        <AdvancedSelector
          options={options}
          selectedOptions={selectedOptions}
          hasNextPage={hasNextPage}
          isNextPageLoading={isNextPageLoading}
          loadNextPage={this.loadNextPage}
          size={select("size", sizes, "full")}
          placeholder={text("placeholder", "Search users")}
          onSearchChanged={value => {
            action("onSearchChanged")(value);
            /*set(
            options.filter(option => {
              return option.label.indexOf(value) > -1;
            })
          );*/
          }}
          groups={groups}
          selectedGroups={selectedGroups}
          isMultiSelect={boolean("isMultiSelect", true)}
          buttonLabel={text("buttonLabel", "Add members")}
          selectAllLabel={text("selectAllLabel", "Select all")}
          onSelect={selectedOptions => {
            action("onSelect")(selectedOptions);
            this.toggle();
          }}
          //onCancel={toggle}
          onGroupSelect={selectedGroups => {
            action("onGroupSelect")(selectedGroups);
            /*set(
            options.filter(option => {
              return (
                option.groups &&
                option.groups.length > 0 &&
                option.groups.indexOf(group.key) > -1
              );
            })
          );*/
          }}
          onGroupChange={group => {
            action("onGroupChange")(group);
            /*set(
            options.filter(option => {
              return (
                option.groups &&
                option.groups.length > 0 &&
                option.groups.indexOf(group.key) > -1
              );
            })
          );*/
          }}
          allowCreation={boolean("allowCreation", false)}
          onAddNewClick={() => action("onSelect")}
          isOpen={isOpen}
          displayType={select("displayType", displayTypes, "dropdown")}
        />
      </div>
    );
  }
}

storiesOf("Components|AdvancedSelector", module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add("base", () => {
    return (
      <Section>
        <ADSelectorExample isOpen={boolean("isOpen", true)} total={number("Users count", 10000)} />
      </Section>
    );
  });
