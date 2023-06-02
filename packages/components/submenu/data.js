import React from "react";
import FileInput from "@docspace/components/file-input";
import Row from "@docspace/components/row";
import Textarea from "@docspace/components/textarea";
import Text from "../text";

export const data = [
  {
    id: "Overview",
    name: "Overview",
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
    content: <p>Documents</p>,
  },
  {
    id: "Milestones",
    name: "Milestones",
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
        <div
          style={{
            alignItems: "center",
            justifyContent: "space-between",
            display: "flex",
          }}
        >
          <Text truncate>Sample text</Text>
        </div>
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
