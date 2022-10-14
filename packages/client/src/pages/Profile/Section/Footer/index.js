import React from "react";
import Text from "@docspace/components/text";
import Link from "@docspace/components/link";
import Box from "@docspace/components/box";
import HelpButton from "@docspace/components/help-button";
import { ReactSVG } from "react-svg";
import {
  StyledFooter,
  Table,
  TableHead,
  TableRow,
  TableHeaderCell,
  TableBody,
  TableDataCell,
} from "./styled-active-sessions";

const sessionRemove = <ReactSVG wrapper="span" src="images/remove.react.svg" />;

const sessions = [
  {
    name: "Windows 10",
    desc: "(Chrome 100)",
    date: "5/4/2022, 1:24 PM",
    ip: "108.101.45.223, 171.18.0.9",
  },
  {
    name: "iOS 15 Apple iPhone",
    desc: "(Mobile Safari 15)",
    date: "5/4/2022, 1:24 PM",
    ip: "75.100.45.223, 171.18.0.9",
  },
  {
    name: "Windows 8",
    desc: "(Safari)",
    date: "5/4/2022, 1:24 PM",
    ip: "83.15.45.223, 171.18.0.9",
  },
];

const onClickLogoutSession = () => {
  console.log("logout");
};

const onClickRemoveSession = () => {
  console.log("remove session");
};

const ActiveSessions = () => {
  return (
    <StyledFooter>
      <Text fontSize="16px" fontWeight={700}>
        Active Sessions
      </Text>
      <Box
        displayProp="flex"
        alignItems="center"
        justifyContent="flex-start"
        marginProp="10px 0 0"
      >
        <Link
          className="session-logout"
          type="action"
          isHovered
          onClick={onClickLogoutSession}
        >
          Log out from all active sessions
        </Link>
        <HelpButton
          iconName="/static/images/info.react.svg"
          tooltipContent={
            <Text fontSize="13px">Paste you tooltip content here</Text>
          }
        />
      </Box>

      <Table>
        <TableHead>
          <TableRow>
            <TableHeaderCell>Sessions</TableHeaderCell>
            <TableHeaderCell>Date</TableHeaderCell>
            <TableHeaderCell>IP-address</TableHeaderCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {sessions.map((session) => (
            <TableRow key={session.ip}>
              <TableDataCell>
                {session.name}
                <span>{session.desc}</span>
              </TableDataCell>
              <TableDataCell>{session.date}</TableDataCell>
              <TableDataCell>{session.ip}</TableDataCell>
              <TableDataCell onClick={onClickRemoveSession}>{sessionRemove}</TableDataCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </StyledFooter>
  );
};

export default ActiveSessions;
