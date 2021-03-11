import React from "react";
import RowContainer from ".";
import Row from "../row";
import RowContent from "../row-content";
import Avatar from "../avatar";
import Link from "../link";
import SendClockIcon from "../public/static/images/send.clock.react.svg";
import CatalogSpamIcon from "../public/static/images/catalog.spam.react.svg";

export default {
  title: "Components/RowContainer",
  component: RowContainer,
  subcomponents: { Row, RowContent },
  parameters: {
    docs: { description: { component: "Container for Row component" } },
  },
};

const getRndString = (n) =>
  Math.random()
    .toString(36)
    .substring(2, n + 2);

const getRndNumber = (a, b) => Math.floor(Math.random() * (b - a)) + a;

const getRndBool = () => Math.random() >= 0.5;

const fillFakeData = (n) => {
  const data = [];

  for (let i = 0; i < n; i++) {
    data.push({
      id: getRndString(6),
      userName:
        getRndString(getRndNumber(2, 6)) +
        " " +
        getRndString(getRndNumber(2, 10)),
      avatar: "",
      role: getRndBool()
        ? "user"
        : getRndBool()
        ? "guest"
        : getRndBool()
        ? "admin"
        : "owner",
      status: getRndBool() ? "normal" : getRndBool() ? "disabled" : "pending",
      isHead: getRndBool(),
      department: getRndBool() ? "Demo department" : "",
      mobilePhone: "+" + getRndNumber(10000000000, 99999999999),
      email: getRndString(12) + "@yahoo.com",
      contextOptions: [
        { key: 1, label: getRndString(6) },
        { key: 2, label: getRndString(5) },
        { key: 3, label: getRndString(4) },
        { key: 4, label: getRndString(3) },
        { key: 5, label: getRndString(2) },
        { key: 6, label: getRndString(1) },
      ],
    });
  }

  return data;
};

const fakeData = fillFakeData(20);

const Template = (args) => {
  return (
    <RowContainer {...args} manualHeight="500px">
      {fakeData.map((user) => {
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
                  containerWidth="120px"
                  type="page"
                  title="Head of department"
                  fontSize="12px"
                  color={sideInfoColor}
                >
                  Head of department
                </Link>
              ) : (
                <div></div>
              )}
              <Link
                containerWidth="160px"
                type="action"
                title={user.department}
                fontSize="12px"
                color={sideInfoColor}
              >
                {user.department}
              </Link>
              <Link
                type="page"
                title={user.mobilePhone}
                fontSize="12px"
                color={sideInfoColor}
              >
                {user.mobilePhone}
              </Link>
              <Link
                containerWidth="180px"
                type="page"
                title={user.email}
                fontSize="12px"
                color={sideInfoColor}
              >
                {user.email}
              </Link>
            </RowContent>
          </Row>
        );
      })}
    </RowContainer>
  );
};

export const Default = Template.bind({});
