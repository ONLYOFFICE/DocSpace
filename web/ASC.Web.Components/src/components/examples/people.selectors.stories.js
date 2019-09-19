import React from "react";
import { storiesOf } from "@storybook/react";
import { action } from "@storybook/addon-actions";
import { withKnobs, text } from "@storybook/addon-knobs/react";
import AdvancedSelector from "../advanced-selector";
import Section from "../../../.storybook/decorators/section";
import { boolean, number } from "@storybook/addon-knobs/dist/deprecated";
import { ArrayValue, BooleanValue } from "react-values";
import Button from "../button";
import ModalDialog from "../modal-dialog";
import FieldContainer from "../field-container";
import TextInput from "../text-input";
import ComboBox from "../combobox";
import ComboButton from '../combobox/sub-components/combo-button'

const groups = [
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
  },
  {
    key: "group-1",
    label: "Group1",
    total: 0
  },
  {
    key: "group-2",
    label: "Group2",
    total: 0
  },
  {
    key: "group-3",
    label: "Group3",
    total: 0
  },
  {
    key: "group-4",
    label: "Group4",
    total: 0
  },
  {
    key: "group-5",
    label: "Group5",
    total: 0
  }
];

function getRandomInt(min, max) {
  return Math.floor(Math.random() * (max - min)) + min;
}

storiesOf("EXAMPLES|AdvancedSelector", module)
  .addDecorator(withKnobs)
  // To set a default viewport for all the stories for this component
  .addParameters({ viewport: { defaultViewport: "responsive" } })
  .add("people group selector", () => {
    return (
      <Section>
        <BooleanValue
          defaultValue={true}
          onChange={() => action("isOpen changed")}
        >
          {({ value: isOpen, toggle }) => (
            <div style={{ position: "relative" }}>
              <Button label="Toggle dropdown" onClick={toggle} />
              <ArrayValue
                defaultValue={groups}
                onChange={() => action("options onChange")}
              >
                {({ value, set }) => (
                  <AdvancedSelector
                    isDropDown={true}
                    isOpen={isOpen}
                    size="compact"
                    placeholder={text("placeholder", "Search")}
                    onSearchChanged={value => {
                      action("onSearchChanged")(value);
                      set(
                        groups.filter(option => {
                          return option.label.indexOf(value) > -1;
                        })
                      );
                    }}
                    options={value}
                    isMultiSelect={boolean("isMultiSelect", true)}
                    buttonLabel={text("buttonLabel", "Add departments")}
                    selectAllLabel={text("selectAllLabel", "Select all")}
                    onSelect={selectedOptions => {
                      action("onSelect")(selectedOptions);
                      toggle();
                    }}
                    onCancel={toggle}
                  />
                )}
              </ArrayValue>
            </div>
          )}
        </BooleanValue>
      </Section>
    );
  })
  .add("people user selector", () => {
    const optionsCount = number("Users count", 1000);
    const options = Array.from({ length: optionsCount }, (v, index) => {
      const additional_group = groups[getRandomInt(1, 6)];
      groups[0].total++;
      additional_group.total++;
      return {
        key: `user${index}`,
        groups: ["group-all", additional_group.key],
        label: `User${index + 1} (All groups, ${additional_group.label})`
      };
    });
    return (
      <Section>
        <BooleanValue
          defaultValue={false}
          onChange={() => action("modalVisible changed")}
        >
          {({ value: modalVisible, toggle: toggleModalVisible }) => (
            <BooleanValue
              defaultValue={true}
              onChange={() => action("isOpen changed")}
            >
              {({ value: isOpen, toggle }) => (
                <div style={{ position: "relative" }}>
                  <Button label="Toggle dropdown" onClick={toggle} />
                  <ArrayValue
                    defaultValue={options}
                    onChange={() => action("options onChange")}
                  >
                    {({ value, set }) => (
                      <>
                        <AdvancedSelector
                          isDropDown={true}
                          isOpen={isOpen}
                          size="full"
                          placeholder={text("placeholder", "Search")}
                          onSearchChanged={value => {
                            action("onSearchChanged")(value);
                            set(
                              options.filter(option => {
                                return option.label.indexOf(value) > -1;
                              })
                            );
                          }}
                          options={value}
                          groups={groups}
                          isMultiSelect={boolean("isMultiSelect", true)}
                          buttonLabel={text("buttonLabel", "Add departments")}
                          selectAllLabel={text("selectAllLabel", "Select all")}
                          onSelect={selectedOptions => {
                            action("onSelect")(selectedOptions);
                            toggle();
                          }}
                          onCancel={toggle}
                          allowCreation={boolean("allowCreation", true)}
                          onAddNewClick={toggleModalVisible}
                          allowAnyClickClose={!modalVisible}
                        />
                        <ModalDialog
                          zIndex={1001}
                          visible={modalVisible}
                          headerContent="New User"
                          bodyContent={
                            <div className="create_new_user_modal">
                              <FieldContainer
                                isVertical={true}
                                isRequired={true}
                                hasError={false}
                                labelText={"First name:"}
                              >
                                <TextInput
                                  value={""}
                                  hasError={false}
                                  className="firstName-input"
                                  scale={true}
                                  autoComplete="off"
                                  onChange={e => {
                                    //set(e.target.value);
                                  }}
                                />
                              </FieldContainer>
                              <FieldContainer
                                isVertical={true}
                                isRequired={true}
                                hasError={false}
                                labelText={"Last name:"}
                              >
                                <TextInput
                                  value={""}
                                  hasError={false}
                                  className="lastName-input"
                                  scale={true}
                                  autoComplete="off"
                                  onChange={e => {
                                    //set(e.target.value);
                                  }}
                                />
                              </FieldContainer>
                              <FieldContainer
                                isVertical={true}
                                isRequired={true}
                                hasError={false}
                                labelText={"E-mail:"}
                              >
                                <TextInput
                                  value={""}
                                  hasError={false}
                                  className="email-input"
                                  scale={true}
                                  autoComplete="off"
                                  onChange={e => {
                                    //set(e.target.value);
                                  }}
                                />
                              </FieldContainer>
                              <FieldContainer
                                isVertical={true}
                                isRequired={true}
                                hasError={false}
                                labelText={"Group:"}
                              >
                                <ComboBox
                                  options={groups}
                                  className="group-input"
                                  onSelect={option =>
                                    console.log("Selected option", option)
                                  }
                                  selectedOption={{
                                    key: 0,
                                    label: "Select"
                                  }}
                                  dropDownMaxHeight={200}
                                  scaled={true}
                                  scaledOptions={true}
                                  size="content"
                                />
                              </FieldContainer>
                            </div>
                          }
                          footerContent={[
                            <Button
                              key="CreateBtn"
                              label="Create"
                              primary={true}
                              size="big"
                              onClick={e => {
                                console.log("CreateBtn click", e);
                                toggleModalVisible();
                              }}
                            />
                          ]}
                          onClose={toggleModalVisible}
                        />
                      </>
                    )}
                  </ArrayValue>
                </div>
              )}
            </BooleanValue>
          )}
        </BooleanValue>
      </Section>
    );
  })
  .add("people user selector as advanced ComboBox", () => {
    const optionsCount = number("Users count", 1000);
    const options = Array.from({ length: optionsCount }, (v, index) => {
      const additional_group = groups[getRandomInt(1, 6)];
      groups[0].total++;
      additional_group.total++;
      return {
        key: `user${index}`,
        groups: ["group-all", additional_group.key],
        label: `User${index + 1} (All groups, ${additional_group.label})`
      };
    });
    let selectedOptionsHead = [];
    const getSelectedOption = (count, maxCount) => {
      return {
        key: 0,
        //icon: 'icon_name', //if you need insert ComboBox styled image 
        label: `Selected (${count}/${maxCount})`
      }
    }

    return (
      <Section>
        <BooleanValue
          defaultValue={false}
          onChange={() => action("modalVisible changed")}
        >
          {({ value: modalVisible, toggle: toggleModalVisible }) => (
            <BooleanValue
              defaultValue={true}
              onChange={() => action("isOpen changed")}
            >
              {({ value: isOpen, toggle }) => (
                <div style={{ position: "relative" }}>
                  <ComboButton
                    label="Toggle dropdown"
                    options={options}
                    isOpen={isOpen}
                    selectedOption={getSelectedOption(selectedOptionsHead.length, options.length)}
                    //innerContainer={react.node} // if you need insert custom node element inside ComboBox
                    onClick={toggle}
                  />
                  <ArrayValue
                    defaultValue={options}
                    onChange={() => action("options onChange")}
                  >
                    {({ value, set }) => (
                      <>
                        <AdvancedSelector
                          isDropDown={true}
                          isOpen={isOpen}
                          size="full"
                          placeholder={text("placeholder", "Search")}
                          onSearchChanged={value => {
                            action("onSearchChanged")(value);
                            set(
                              options.filter(option => {
                                return option.label.indexOf(value) > -1;
                              })
                            );
                          }}
                          options={value}
                          groups={groups}
                          isMultiSelect={boolean("isMultiSelect", true)}
                          buttonLabel={text("buttonLabel", "Add departments")}
                          selectAllLabel={text("selectAllLabel", "Select all")}
                          onSelect={selectedOptions => {
                            action("onSelect")(selectedOptions);
                            selectedOptionsHead = selectedOptions;
                            toggle();
                          }}
                          onCancel={toggle}
                          allowCreation={boolean("allowCreation", true)}
                          onAddNewClick={toggleModalVisible}
                          allowAnyClickClose={!modalVisible}
                        />
                        <ModalDialog
                          zIndex={1001}
                          visible={modalVisible}
                          headerContent="New User"
                          bodyContent={
                            <div className="create_new_user_modal">
                              <FieldContainer
                                isVertical={true}
                                isRequired={true}
                                hasError={false}
                                labelText={"First name:"}
                              >
                                <TextInput
                                  value={""}
                                  hasError={false}
                                  className="firstName-input"
                                  scale={true}
                                  autoComplete="off"
                                  onChange={e => {
                                    //set(e.target.value);
                                  }}
                                />
                              </FieldContainer>
                              <FieldContainer
                                isVertical={true}
                                isRequired={true}
                                hasError={false}
                                labelText={"Last name:"}
                              >
                                <TextInput
                                  value={""}
                                  hasError={false}
                                  className="lastName-input"
                                  scale={true}
                                  autoComplete="off"
                                  onChange={e => {
                                    //set(e.target.value);
                                  }}
                                />
                              </FieldContainer>
                              <FieldContainer
                                isVertical={true}
                                isRequired={true}
                                hasError={false}
                                labelText={"E-mail:"}
                              >
                                <TextInput
                                  value={""}
                                  hasError={false}
                                  className="email-input"
                                  scale={true}
                                  autoComplete="off"
                                  onChange={e => {
                                    //set(e.target.value);
                                  }}
                                />
                              </FieldContainer>
                              <FieldContainer
                                isVertical={true}
                                isRequired={true}
                                hasError={false}
                                labelText={"Group:"}
                              >
                                <ComboBox
                                  options={groups}
                                  className="group-input"
                                  onSelect={option =>
                                    console.log("Selected option", option)
                                  }
                                  selectedOption={{
                                    key: 0,
                                    label: "Select"
                                  }}
                                  dropDownMaxHeight={200}
                                  scaled={true}
                                  size="content"
                                  scaledOptions={true}
                                />
                              </FieldContainer>
                            </div>
                          }
                          footerContent={[
                            <Button
                              key="CreateBtn"
                              label="Create"
                              primary={true}
                              size="big"
                              onClick={e => {
                                console.log("CreateBtn click", e);
                                toggleModalVisible();
                              }}
                            />
                          ]}
                          onClose={toggleModalVisible}
                        />
                      </>
                    )}
                  </ArrayValue>
                </div>
              )}
            </BooleanValue>
          )}
        </BooleanValue>
      </Section>
    );
  });
