import React from "react";

import Row from "../row";
import Avatar from "../avatar";
import Link from "../link";
import RowContent from "../row-content";
import SendClockIcon from "../public/static/images/send.clock.react.svg";
import CatalogSpamIcon from "../public/static/images/catalog.spam.react.svg";

export default {
  title: "Examples/Row",
  component: Row,
  subcomponents: { Avatar, Link, RowContent },
  parameters: { docs: { description: { component: "Example" } } },
};

const fakeUsers = [
  {
    id: "1",
    userName: "Helen Walton",
    avatar: "",
    role: "owner",
    status: "normal",
    isHead: false,
    department: "Administration",
    mobilePhone: "+5 104 6473420",
    email: "percival1979@yahoo.com",
    contextOptions: [
      {
        key: "key1",
        label: "Send e-mail",
        onClick: () => console.log("Context action: Send e-mail"),
      },
      {
        key: "key2",
        label: "Send message",
        onClick: () => console.log("Context action: Send message"),
      },
      { key: "key3", isSeparator: true },
      {
        key: "key4",
        label: "Edit",
        onClick: () => console.log("Context action: Edit"),
      },
      {
        key: "key5",
        label: "Change password",
        onClick: () => console.log("Context action: Change password"),
      },
      {
        key: "key6",
        label: "Change e-mail",
        onClick: () => console.log("Context action: Change e-mail"),
      },
      {
        key: "key7",
        label: "Disable",
        onClick: () => console.log("Context action: Disable"),
      },
    ],
  },
  {
    id: "2",
    userName: "Nellie Harder",
    avatar: "",
    role: "user",
    status: "normal",
    isHead: true,
    department: "Development",
    mobilePhone: "+1 716 3748605",
    email: "herta.reynol@yahoo.com",
    contextOptions: [],
  },
  {
    id: "3",
    userName: "Alan Mason",
    avatar: "",
    role: "admin",
    status: "normal",
    isHead: true,
    department: "",
    mobilePhone: "+3 956 2064314",
    email: "davin_lindgr@hotmail.com",
    contextOptions: [
      {
        key: "key1",
        label: "Send e-mail",
        onClick: () => console.log("Context action: Send e-mail"),
      },
      {
        key: "key2",
        label: "Send message",
        onClick: () => console.log("Context action: Send message"),
      },
      { key: "key3", isSeparator: true },
      {
        key: "key4",
        label: "Edit",
        onClick: () => console.log("Context action: Edit"),
      },
      {
        key: "key5",
        label: "Change password",
        onClick: () => console.log("Context action: Change password"),
      },
      {
        key: "key6",
        label: "Change e-mail",
        onClick: () => console.log("Context action: Change e-mail"),
      },
      {
        key: "key7",
        label: "Disable",
        onClick: () => console.log("Context action: Disable"),
      },
    ],
  },
  {
    id: "4",
    userName: "Michael Goldstein",
    avatar: "",
    role: "guest",
    status: "normal",
    isHead: false,
    department: "Visitors",
    mobilePhone: "+7 715 6018678",
    email: "fidel_kerlu@hotmail.com",
    contextOptions: [
      {
        key: "key1",
        label: "Send e-mail",
        onClick: () => console.log("Context action: Send e-mail"),
      },
      {
        key: "key2",
        label: "Send message",
        onClick: () => console.log("Context action: Send message"),
      },
      { key: "key3", isSeparator: true },
      {
        key: "key4",
        label: "Edit",
        onClick: () => console.log("Context action: Edit"),
      },
      {
        key: "key5",
        label: "Change password",
        onClick: () => console.log("Context action: Change password"),
      },
      {
        key: "key6",
        label: "Change e-mail",
        onClick: () => console.log("Context action: Change e-mail"),
      },
      {
        key: "key7",
        label: "Disable",
        onClick: () => console.log("Context action: Disable"),
      },
    ],
  },
  {
    id: "5",
    userName: "Robert Gardner Robert Gardner",
    avatar: "",
    role: "user",
    status: "pending",
    isHead: false,
    department: "",
    mobilePhone: "",
    email: "robert_gardner@hotmail.com",
    contextOptions: [
      {
        key: "key1",
        label: "Edit",
        onClick: () => console.log("Context action: Edit"),
      },
      {
        key: "key2",
        label: "Invite again",
        onClick: () => console.log("Context action: Invite again"),
      },
      {
        key: "key3",
        label: "Delete profile",
        onClick: () => console.log("Context action: Delete profile"),
      },
    ],
  },
  {
    id: "6",
    userName1: "Timothy Morphis",
    avatar: "",
    role: "user",
    status: "disabled",
    isHead: false,
    department: "Disabled",
    mobilePhone: "",
    email: "timothy_j_morphis@hotmail.com",
    contextOptions: [
      {
        key: "key1",
        label: "Edit",
        onClick: () => console.log("Context action: Edit"),
      },
      {
        key: "key2",
        label: "Reassign data",
        onClick: () => console.log("Context action: Reassign data"),
      },
      {
        key: "key3",
        label: "Delete personal data",
        onClick: () => console.log("Context action: Delete personal data"),
      },
      {
        key: "key4",
        label: "Delete profile",
        onClick: () => console.log("Context action: Delete profile"),
      },
    ],
  },
];

const Template = (args) => {
  return (
    <>
      {fakeUsers.map((user) => {
        const element = (
          <Avatar
            size="min"
            role={user.role}
            userName={user.userName}
            source={user.avatar}
          />
        );
        const nameColor = user.status === "pending" ? "#A3A9AE" : "#333333";
        const sideInfoColor = user.status === "pending" ? "#D0D5DA" : "#A3A9AE";

        return (
          <Row
            key={user.id}
            status={user.status}
            checked={false}
            data={user}
            element={element}
            contextOptions={user.contextOptions}
          >
            <RowContent>
              <Link
                type="page"
                title={user.userName}
                isBold={true}
                fontSize="15px"
                color={nameColor}
                isTextOverflow={true}
              >
                {user.userName}
              </Link>
              <>
                {user.status === "pending" && (
                  <SendClockIcon size="small" isfill={true} color="#3B72A7" />
                )}
                {user.status === "disabled" && (
                  <CatalogSpamIcon size="small" isfill={true} color="#3B72A7" />
                )}
              </>
              {user.isHead ? (
                <Link
                  containerWidth="110px"
                  type="page"
                  title="Head of department"
                  fontSize="12px"
                  color={sideInfoColor}
                  isTextOverflow={true}
                >
                  Head of department
                </Link>
              ) : (
                <div style={{ width: "110px" }}></div>
              )}
              <Link
                type="action"
                title={user.department}
                fontSize="12px"
                color={sideInfoColor}
                isTextOverflow={true}
              >
                {user.department}
              </Link>
              <Link
                type="page"
                title={user.mobilePhone}
                fontSize="12px"
                color={sideInfoColor}
                isTextOverflow={true}
              >
                {user.mobilePhone}
              </Link>
              <Link
                containerWidth="220px"
                type="page"
                title={user.email}
                fontSize="12px"
                color={sideInfoColor}
                isTextOverflow={true}
              >
                {user.email}
              </Link>
            </RowContent>
          </Row>
        );
      })}
    </>
  );
};

export const people = Template.bind({});
