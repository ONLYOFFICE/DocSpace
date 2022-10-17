import React from "react";
import Text from "@appserver/components/text";
import Link from "@appserver/components/link";
import IconButton from "@appserver/components/icon-button";
import { ReactSVG } from "react-svg";
import { StyledUserNameLink } from "../../styles/VirtualRoom/history";

const HistoryBlockContent = ({ t, action, details }) => {
  return (
    <div className="block-content">
      {action === "message" ? (
        <Text>{details.message}</Text>
      ) : action === "appointing" ? (
        <div className="appointing">
          {`${t("appointed")} 
            ${details.appointedRole} `}{" "}
          <UserNameLink user={details.appointedUser} />
        </div>
      ) : action === "users" ? (
        <div className="users">
          <div className="user-list">
            added users{" "}
            {details.users.map((user, i) => (
              <UserNameLink
                key={user.id}
                user={user}
                withComma={i + 1 !== details.users.length}
              />
            ))}
          </div>
        </div>
      ) : action === "files" ? (
        <div className="files">
          <Text>add new 2 files into the folder “My project”</Text>
          <div className="files-list">
            {details.files.map((file) => (
              <div className="file" key={file.id}>
                <ReactSVG className="icon" src={file.icon} />
                <div className="file-title">
                  <span className="name">{file.title.split(".")[0]}</span>
                  <span className="exst">{file.fileExst}</span>
                </div>
                <IconButton
                  className="location-btn"
                  iconName="/static/images/folder-location.react.svg"
                  size="16"
                  isFill={true}
                  onClick={() => {}}
                />
              </div>
            ))}
          </div>
        </div>
      ) : null}
    </div>
  );
};

const UserNameLink = ({ user, withComma }) => {
  const username = user.displayName || user.email;
  const space = <div className="space"></div>;

  return (
    <StyledUserNameLink className="user">
      {user.profileUrl ? (
        <Link
          className="username link"
          isHovered
          type="action"
          href={user.profileURl}
        >
          {username}
          {withComma ? "," : ""}
          {withComma && space}
        </Link>
      ) : (
        <div className="username text" key={user.id}>
          {username}
          {withComma ? "," : ""}
          {withComma && space}
        </div>
      )}
    </StyledUserNameLink>
  );
};

export default HistoryBlockContent;
