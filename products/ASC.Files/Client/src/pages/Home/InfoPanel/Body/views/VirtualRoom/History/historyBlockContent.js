import React from "react";
import Text from "@appserver/components/text";
import Link from "@appserver/components/link";

const HistoryBlockContent = ({ t, action, details }) => {
  return (
    <>
      {action === "message" ? (
        <Text>{details.message}</Text>
      ) : action === "appointing" ? (
        <Text>
          {`${t("appointed")} 
            ${details.appointedRole} 
            ${details.appointedUser.displayName}`}
        </Text>
      ) : action === "users" ? (
        <>
          added users
          {details.users.map((user) =>
            user.profileUrl ? (
              <Link
                className="username link"
                type="action"
                href={user.profileURl}
              >
                {user.displayName || user.email}
              </Link>
            ) : (
              <Text className="username text">
                {user.displayName || user.email}
              </Text>
            )
          )}
        </>
      ) : action === "files" ? (
        <HistoryBlockFileActionContent
          type={details.type}
          files={details.files}
        />
      ) : null}
    </>
  );
};

const HistoryBlockFileActionContent = () => {
  return <></>;
};

export default HistoryBlockContent;
