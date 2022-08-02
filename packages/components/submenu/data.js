import React from "react";
import FileInput from "@docspace/components/file-input";
import Row from "@docspace/components/row";
import Textarea from "@docspace/components/textarea";
import Text from "../text";

export const data = [
  {
    id: "Overview",
    name: "Overview",
    onClick: function () {
      alert("Overview");
    },
    content: (
      <FileInput
        accept=".doc, .docx"
        id="file-input-id"
        name="demoFileInputName"
        onInput={() => {}}
        placeholder="Input file"
      />
    ),
  },
  {
    id: "Documents",
    name: "Documents",
    onClick: function () {
      alert("Documents");
    },
    content: (
      <Textarea
        onChange={() => {}}
        value="Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim. Donec pede justo, fringilla vel, aliquet nec, vulputate eget, arcu. In enim justo, rhoncus ut, imperdiet a, venenatis vitae, justo. Nullam dictum felis eu pede mollis pretium. Integer tincidunt. Cras dapibus. Vivamus elementum semper nisi. Aenean vulputate eleifend tellus. Aenean leo ligula, porttitor eu, consequat vitae"
      />
    ),
  },
  {
    id: "Milestones",
    name: "Milestones",
    onClick: function () {
      alert("Milestones");
    },
    content: (
      <Row
        key="1"
        checked
        contextOptions={[
          {
            key: "key1",
            label: "Edit",
            onClick: () => {},
          },
          {
            key: "key2",
            label: "Delete",
            onClick: function noRefCheck() {},
          },
        ]}
        onRowClick={function noRefCheck() {}}
        onSelect={function noRefCheck() {}}
      >
        <Text truncate>Sample text</Text>
      </Row>
    ),
  },
  {
    id: "Time tracking",
    name: "Time tracking",
    content: <p>Time tracking</p>,
  },
  {
    id: "Contacts",
    name: "Contacts",
    content: <p>Contacts</p>,
  },
  {
    id: "Team",
    name: "Team",
    content: <p>Team</p>,
  },
];

export const startSelect = data[2];

export const testData = [
  {
    id: "Tab1",
    name: "Tab1",
    content: <p>1</p>,
  },
  {
    id: "Tab2",
    name: "Tab2",
    content: <p>2</p>,
  },
];

export const testStartSelect = testData[1];
