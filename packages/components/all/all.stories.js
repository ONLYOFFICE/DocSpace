/* eslint-disable react/no-unescaped-entities */
import React from "react";
import { BooleanValue, StringValue } from "react-values";
import Avatar from "../avatar";
import Button from "../button";
//import HelpButton from "../help-button";
//import IconButton from "../icon-button";
import ToggleButton from "../toggle-button";
import Calendar from "../calendar";
import Checkbox from "../checkbox";
import ComboBox from "../combobox";
import InputBlock from "../input-block";
import RadioButtonGroup from "../radio-button-group";
import TextInput from "../text-input";
import Textarea from "../textarea";
//import ContextMenuButton from "../context-menu-button";
// import DatePicker from "../date-picker";
import FieldContainer from "../field-container";
import Heading from "../heading";
import Link from "../link";
import Loader from "../loader";
import Row from "../row";
import Scrollbar from "../scrollbar";
import TabContainer from "../tabs-container";
import Text from "../text";
import Toast from "../toast";
import toastr from "../toast/toastr";
import ToggleContent from "../toggle-content";
import Tooltip from "../tooltip";
import SettingsReactSvg from "PUBLIC_DIR/images/settings.react.svg";
import CatalogFolderReactSvg from "PUBLIC_DIR/images/catalog.folder.react.svg";
import CatalogEmployeeReactSvgUrl from "PUBLIC_DIR/images/catalog.employee.react.svg?url";
import ItemActiveReactSvgUrl from "PUBLIC_DIR/images/item.active.react.svg?url";
import SearchReactSvgUrl from "PUBLIC_DIR/images/search.react.svg?url";
import VerticalDotsReactSvgUrl from "PUBLIC_DIR/images/vertical-dots.react.svg";

const array_items = [
  {
    key: "0",
    title: "Tab 1",
    content: <div>Tab 1 content</div>,
  },
  {
    key: "1",
    title: "Tab 2",
    content: <div>Tab 2 content</div>,
  },
  {
    key: "2",
    title: "Tab 3",
    content: <div>Tab 3 content</div>,
  },
];

const options = [
  {
    key: 0,
    icon: CatalogEmployeeReactSvgUrl, // optional item
    label: "Option 1",
    disabled: false, // optional item
    onClick: () => {}, // optional item
  },
  {
    key: 1,
    icon: CatalogEmployeeReactSvgUrl, // optional item
    label: "Option 2",
    disabled: false, // optional item
    onClick: () => {}, // optional item
  },
  {
    key: 2,
    icon: CatalogEmployeeReactSvgUrl, // optional item
    label: "Option 3",
    disabled: true, // optional item
    onClick: () => {}, // optional item
  },
  {
    key: 3,
    icon: CatalogEmployeeReactSvgUrl, // optional item
    label: "Option 4",
    disabled: false, // optional item
    onClick: () => {}, // optional item
  },
];

const arrayUsers = [
  { key: "user_1", name: "Bob", email: "Bob@gmail.com", position: "developer" },
  {
    key: "user_2",
    name: "John",
    email: "John@gmail.com",
    position: "developer",
  },
  {
    key: "user_3",
    name: "Kevin",
    email: "Kevin@gmail.com",
    position: "developer",
  },
];

const element = "Icon";

const elementAvatar = <Avatar size="min" role="user" userName="Demo Avatar" />;
const elementIcon = <CatalogFolderReactSvg size="big" />;
const elementComboBox = (
  <ComboBox
    options={[
      { key: 1, icon: ItemActiveReactSvgUrl, label: "Open" },
      { key: 2, icon: "CheckIcon", label: "Closed" },
    ]}
    onSelect={(option) => console.log(option)}
    selectedOption={{
      key: 0,
      icon: ItemActiveReactSvgUrl,
      label: "",
    }}
    scaled={false}
    size="content"
    isDisabled={false}
  />
);

const checkedProps = { checked: false };
const getElementProps = (element) =>
  element === "Avatar"
    ? { element: elementAvatar }
    : element === "Icon"
    ? { element: elementIcon }
    : element === "ComboBox"
    ? { element: elementComboBox }
    : {};

const elementProps = getElementProps(element);

const rowContent = (
  <>
    <Row
      key="1"
      {...checkedProps}
      {...elementProps}
      contextOptions={[
        {
          key: "key1",
          label: "Edit",
          onClick: () => console.log("Context action: Edit"),
        },
        {
          key: "key2",
          label: "Delete",
          onClick: () => console.log("Context action: Delete"),
        },
      ]}
    >
      <Text truncate={true}>Sample text</Text>
    </Row>
  </>
);

let rowCount = 5;
const rowArray = [];
while (rowCount != 0) {
  rowArray.push(rowContent);
  rowCount--;
}

export default {
  title: "All",
  parameters: {
    docs: { description: { component: "All components" } },
  },
};
const Template = (args) => (
  <>
    <div
      style={{
        display: "flex",
        flexWrap: "wrap",
        gap: "25px",
      }}
    >
      <div style={{ justifySelf: "center" }}>
        <div style={{ padding: "8px 0" }}>
          <Heading level={1} title="Some title">
            Heading text
          </Heading>
        </div>
        <div style={{ padding: "8px 0" }}>
          <Text as="p" title="Some title">
            Text as "p"
          </Text>
        </div>

        <div style={{ padding: "8px 0" }}>
          <Link type="page" href="https://github.com">
            Black page link
          </Link>
          <br />
          <Link type="page" href="https://github.com" isHovered={true}>
            Black hovered page link
          </Link>
          <br />
          <Link type="action">Black action link</Link>
          <br />
          <Link type="action" isHovered={true}>
            Black hovered action link
          </Link>
        </div>
        <div style={{ padding: "24px 0 8px 0" }}>
          <Link data-for="group" data-tip={0}>
            Bob
          </Link>
          <br />
          <Link data-for="group" data-tip={1}>
            John
          </Link>
          <br />
          <Link data-for="group" data-tip={2}>
            Kevin
          </Link>
          <Tooltip
            id="group"
            offsetRight={90}
            getContent={(dataTip) =>
              dataTip ? (
                <div>
                  <Text isBold={true} fontSize="16px">
                    {arrayUsers[dataTip].name}
                  </Text>
                  <Text color="#A3A9AE" fontSize="13px">
                    {arrayUsers[dataTip].email}
                  </Text>
                  <Text fontSize="13px">{arrayUsers[dataTip].position}</Text>
                </div>
              ) : null
            }
          />
        </div>
        <div style={{ padding: "8px 0" }}>
          <div style={{ display: "flex" }}>
            <div style={{ marginRight: 16 }}>
              <Button
                size="normal"
                isDisabled={false}
                onClick={() => {}}
                label="Button"
                primary
              />
            </div>

            <Button
              size="normal"
              isDisabled={false}
              onClick={() => {}}
              label="Button"
            />
          </div>
        </div>
        <div style={{ padding: "8px 0" }}>
          <Toast />
          <Button
            label="Show toastr"
            onClick={() =>
              toastr.success("Some text for toast", "Some text for title", true)
            }
          />
        </div>
        {/*
            <div style={{ padding: "8px 0" }}>
            <ContextMenuButton
              iconName={VerticalDotsReactSvgUrl}
              size={16}
              color="#A3A9AE"
              isDisabled={false}
              title="Actions"
              getData={() => [
                {
                  key: "key",
                  label: "label",
                  onClick: () => {}
                }
              ]}
            />
          </div>
          */}
        {/*<div style={{ padding: "8px 0" }}>
            <div style={{ display: "flex" }}>
              <div style={{ marginRight: 16 }}>
                <IconButton
                  size="25"
                  isDisabled={false}
                  onClick={() => {}}
                  iconName={VerticalDotsReactSvgUrl}
                  isFill={true}
                  isClickable={false}
                />
              </div>
              <HelpButton tooltipContent="Paste you tooltip content here" />
            </div>
          </div>
          */}
      </div>
      <div style={{ justifySelf: "center" }}>
        <div style={{ padding: "8px 0" }}>
          <ComboBox
            options={options}
            isDisabled={false}
            selectedOption={{
              key: 0,
              label: "Select",
            }}
            dropDownMaxHeight={200}
            noBorder={false}
            scaledOptions={true}
            size="content"
            onSelect={(option) => console.log("selected", option)}
          />
        </div>

        <div style={{ padding: "8px 0" }}>
          <StringValue>
            {({ value, set }) => (
              <TextInput
                placeholder="Add input text"
                value={value}
                onChange={(e) => set(e.target.value)}
              />
            )}
          </StringValue>
        </div>

        <div style={{ padding: "8px 0" }}>
          <StringValue>
            {({ value, set }) => (
              <InputBlock
                placeholder="Add input text"
                iconName={SearchReactSvgUrl}
                onIconClick={() => {}}
                onChange={(e) => set(e.target.value)}
                value={value}
              >
                <SettingsReactSvg size="medium" />
              </InputBlock>
            )}
          </StringValue>
        </div>

        {/* <div style={{ padding: "8px 0" }}>
          <DatePicker
            onChange={(date) => {
              console.log("Selected date", date);
            }}
            selectedDate={new Date()}
            minDate={new Date("1970/01/01")}
            maxDate={new Date(new Date().getFullYear() + 1 + "/01/01")}
            isDisabled={false}
            isReadOnly={false}
            hasError={false}
            isOpen={false}
            themeColor="#ED7309"
            locale="en"
            setSelectedDate={(date) => {
              console.log("Selected date", date);
            }}
          />
        </div> */}

        <div style={{ padding: "8px 0" }}>
          <StringValue>
            {({ value, set }) => (
              <Textarea
                placeholder="Add comment"
                onChange={(event) => set(event.target.value)}
                value={value}
              />
            )}
          </StringValue>
        </div>
        <div style={{ padding: "8px 0" }}>
          <StringValue>
            {({ value, set }) => (
              <FieldContainer
                place="top"
                isRequired
                tooltipContent="Paste you tooltip content here"
                labelText="Name:"
              >
                <TextInput
                  value={value}
                  onChange={(e) => set(e.target.value)}
                />
              </FieldContainer>
            )}
          </StringValue>
        </div>
        <div style={{ padding: "8px 0" }}>
          <RadioButtonGroup
            name="fruits"
            selected="banana"
            options={[
              { value: "apple", label: "Sweet apple" },
              { value: "banana", label: "Banana" },
              { value: "Mandarin" },
            ]}
          />
        </div>
        <div style={{ padding: "8px 0" }}>
          <BooleanValue>
            {({ value, toggle }) => (
              <div style={{ display: "flex" }}>
                <div style={{ marginRight: 24 }}>
                  <Checkbox
                    id="id"
                    name="name"
                    value="value"
                    label="Checkbox"
                    isChecked={value}
                    isIndeterminate={false}
                    isDisabled={false}
                    onChange={(e) => toggle(e.target.checked)}
                  />
                </div>
                <Checkbox
                  id="id"
                  name="name"
                  value="value"
                  label="Checkbox indeterminate"
                  isIndeterminate={true}
                  isDisabled={false}
                  onChange={() => {}}
                />
              </div>
            )}
          </BooleanValue>
        </div>

        <div style={{ padding: "8px 0 24px 0" }}>
          <BooleanValue>
            {({ value, toggle }) => (
              <ToggleButton
                label="Toggle button"
                onChange={(e) => toggle(e.target.checked)}
                isChecked={value}
              />
            )}
          </BooleanValue>
        </div>
      </div>
      <div style={{ justifySelf: "center" }}>
        <div style={{ padding: "8px 0" }}>
          <Calendar
            onChange={() => {}}
            disabled={false}
            themeColor="#ED7309"
            selectedDate={new Date()}
            openToDate={new Date()}
            minDate={new Date("1970/01/01")}
            maxDate={new Date("3000/01/01")}
            locale="ru"
            setSelectedDate={(date) => {}}
          />
        </div>
      </div>
      <div style={{ justifySelf: "center" }}>
        <div style={{ padding: "8px 0" }}>
          {rowArray[0]}
          {rowArray.map((item, index) => {
            return <div key={index}>{item}</div>;
          })}
        </div>
      </div>

      <div style={{ justifySelf: "center" }}>
        <div style={{ padding: "8px 0" }}>
          <Avatar
            size="max"
            role="admin"
            source=""
            userName=""
            editing={false}
          />
        </div>
      </div>
      <div style={{ justifySelf: "center" }}>
        <div style={{ padding: "8px 0" }}>
          <Loader type="base" color="black" size="30px" label="Loading..." />
        </div>
        <div style={{ padding: "8px 0", marginLeft: 45 }}>
          <Loader type="oval" color="black" size="30px" label="Loading" />
        </div>
        <div style={{ padding: "8px 0", marginLeft: 45 }}>
          <Loader type="dual-ring" color="black" size="30px" label="Loading" />
        </div>
      </div>
      <div style={{ justifySelf: "center" }}>
        <div style={{ padding: "8px 0" }}>
          <Scrollbar stype="mediumBlack" style={{ width: 300, height: 200 }}>
            ================================================================
            Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do
            eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim
            ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut
            aliquip ex ea commodo consequat. Duis aute irure dolor in
            reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla
            pariatur. Excepteur sint occaecat cupidatat non proident, sunt in
            culpa qui officia deserunt mollit anim id est laborum. Lorem ipsum
            dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor
            incididunt ut labore et dolore magna aliqua. Ut enim ad minim
            veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex
            ea commodo consequat. Duis aute irure dolor in reprehenderit in
            voluptate velit esse cillum dolore eu fugiat nulla pariatur.
            Excepteur sint occaecat cupidatat non proident, sunt in culpa qui
            officia deserunt mollit anim id est laborum.
            ================================================================
          </Scrollbar>
        </div>
      </div>
      <div>
        <div style={{ padding: "8px 0" }}>
          <TabContainer elements={array_items} />
        </div>
        <div style={{ padding: "16px 0" }}>
          <ToggleContent label="ToggleContent">
            <span>Toggle content text</span>
          </ToggleContent>
        </div>
      </div>
    </div>
  </>
);

export const All = Template.bind({});
