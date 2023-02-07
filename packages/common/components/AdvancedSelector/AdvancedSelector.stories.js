/* eslint-disable react/prop-types */
import React from "react";
import { storiesOf } from "@storybook/react";
import { action } from "@storybook/addon-actions";
import {
  withKnobs,
  text,
  number,
  boolean,
  select,
} from "@storybook/addon-knobs/react";
import withReadme from "storybook-readme/with-readme";
import Readme from "./README.md";
import AdvancedSelector2 from ".";
import Section from "../../../.storybook/decorators/section";
import Button from "@docspace/components/button";
import equal from "fast-deep-equal/react";
import UserTooltip from "@docspace/client/src/components/PeopleSelector/UserTooltip";

function getRandomInt(min, max) {
  return Math.floor(Math.random() * (max - min)) + min;
}
const sizes = ["compact", "full"];
const displayTypes = ["dropdown", "aside", "auto"];

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
      isNextPageLoading: false,
    };
  };

  generateGroups = () => {
    return [
      {
        key: "group-administration",
        label: "Administration",
        total: 0,
      },
      {
        key: "group-dev",
        label: "Development",
        total: 0,
      },
      {
        key: "group-management",
        label: "Management",
        total: 0,
      },
      {
        key: "group-marketing",
        label: "Marketing",
        total: 0,
      },
      {
        key: "group-mobile",
        label: "Mobile",
        total: 0,
      },
      {
        key: "group-support",
        label: "Support",
        total: 0,
      },
      {
        key: "group-web",
        label: "Web",
        total: 0,
      },
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
        label: `Demo User ${index}`,
        avatarUrl: ``,
        position: `Demo`,
        email: `demo@demo.demo`,
      };
    });
  };

  loadNextPage = ({ startIndex, searchValue, currentGroup }) => {
    console.log(
      `loadNextPage(startIndex=${startIndex}, searchValue="${searchValue}", currentGroup="${currentGroup}")`
    );
    this.setState({ isNextPageLoading: true }, () => {
      setTimeout(() => {
        const { options } = this.state;

        let filtered = [...this.state.allOptions];

        if (currentGroup) {
          filtered = filtered.filter(
            (o) => o.groups.indexOf(currentGroup) > -1
          );
        }

        if (searchValue) {
          filtered = filtered.filter((o) => o.label.indexOf(searchValue) > -1);
        }

        const newOptions = [...options].concat(
          filtered.slice(startIndex, startIndex + 100)
        );

        this.setState({
          hasNextPage: newOptions.length < filtered.length,
          isNextPageLoading: false,
          options: newOptions,
        });
      }, 1000);
    });
  };

  componentDidUpdate(prevProps) {
    const { total, options, isOpen } = this.props;
    if (!equal(prevProps.options, options)) {
      this.setState({
        options: options,
      });
    }

    if (isOpen !== prevProps.isOpen) {
      this.setState({
        isOpen: isOpen,
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
      isOpen: !this.state.isOpen,
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
      isNextPageLoading,
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
          displayType={select("displayType", displayTypes, "auto")}
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
          emptySearchOptionsLabel={text(
            "emptySearchOptionsLabel",
            "There are no users with such name"
          )}
          emptyOptionsLabel={text("emptyOptionsLabel", "There are no users")}
          loadingLabel={text("loadingLabel", "Loading... Please wait...")}
          onSelect={(selectedOptions) => {
            action("onSelect")(selectedOptions);
            this.toggle();
          }}
          onSearchChanged={(value) => {
            action("onSearchChanged")(value);
            this.setState({ options: [], hasNextPage: true });
          }}
          onGroupChanged={(group) => {
            action("onGroupChanged")(group);
            this.setState({ options: [], hasNextPage: true });
          }}
          onCancel={() =>
            this.setState({
              isOpen: false,
            })
          }
          getOptionTooltipContent={(index) => {
            if (!index) return null;

            const user = options[+index];

            if (!user) return null;

            // console.log("onOptionTooltipShow", index, user);

            return (
              <UserTooltip
                avatarUrl={user.avatarUrl}
                label={user.label}
                email={user.email}
                position={user.position}
              />
            );
          }}
        />
      </div>
    );
  }
}

storiesOf("Components|AdvancedSelector", module)
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
