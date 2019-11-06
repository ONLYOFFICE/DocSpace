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
import AdvancedSelector2 from "./";
import Section from "../../../.storybook/decorators/section";
import Button from "../button";
import Avatar from "../avatar";
import { Text } from "../text";
import { isEqual, slice } from "lodash";
import { name, image, internet } from "faker";

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
        key: "group-administration",
        label: "Administration",
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
      const additional_group = groups[getRandomInt(0, 6)];
      //groups[0].total++;
      additional_group.total++;
      return {
        key: `user${index}`,
        groups: [additional_group.key],
        label: name.findName(),
        avatarUrl: image.avatar(),
        position: name.jobTitle(),
        email: internet.email()
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
          hasNextPage: options.length < this.props.total,
          isNextPageLoading: false,
          options: newOptions
        });
      }, 1000);
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
      <div style={{ position: "relative" }}>
        <Button label="Toggle dropdown" onClick={this.toggle} />
        <AdvancedSelector2
          options={options}
          groups={groups}
          hasNextPage={hasNextPage}
          isNextPageLoading={isNextPageLoading}
          loadNextPage={this.loadNextPage}
          size={select("size", sizes, "full")}
          displayType={select("displayType", displayTypes, "dropdown")}
          selectedOptions={selectedOptions}
          selectedGroups={selectedGroups}
          isOpen={isOpen}
          isMultiSelect={boolean("isMultiSelect", true)}
          isDisabled={boolean("isDisabled", false)}
          searchPlaceHolderLabel={text(
            "searchPlaceHolderLabel",
            "Search users"
          )}
          selectButtonLabel={text("selectButtonLabel", "Add members")}
          selectAllLabel={text("selectAllLabel", "Select all")}
          groupsHeaderLabel={text("groupsHeaderLabel", "Groups")}
          onSelect={selectedOptions => {
            action("onSelect")(selectedOptions);
            this.toggle();
          }}
          onSearchChanged={value => {
            action("onSearchChanged")(value);
            /*set(
            options.filter(option => {
              return option.label.indexOf(value) > -1;
            })
          );*/
          }}
          getOptionTooltipContent={index => {
            if(!index)
              return null;

            const user = options[+index];

            console.log("onOptionTooltipShow", index, user);

            return (
              <div style={{width: 253, height: 63, display: "grid", gridTemplateColumns: "30px 1fr", gridTemplateRows: "1fr", gridColumnGap: 8 }}>
                <Avatar size="small" role="user" source={user.avatarUrl} userName="" editing={false} />
                <div>
                  <Text.Body isBold={true} fontSize={16}>
                    {user.label}
                  </Text.Body>
                  <Text.Body color="#A3A9AE" fontSize={13} style={{paddingBottom: 8}}>
                    {user.email}
                  </Text.Body>
                  <Text.Body fontSize={13}>
                    {user.position}
                  </Text.Body>
                </div>
              </div>
              );
          }}
        />
      </div>
    );
  }
}

storiesOf("Components|AdvancedSelector2", module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .addParameters({ options: { addonPanelInRight: false } })
  .add("base", () => {
    return (
      <Section>
        <ADSelectorExample
          isOpen={boolean("isOpen", true)}
          total={number("Users count", 10000)}
        />
      </Section>
    );
  });
