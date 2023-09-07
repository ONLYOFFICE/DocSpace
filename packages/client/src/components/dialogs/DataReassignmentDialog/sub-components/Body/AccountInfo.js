import { StyledOwnerInfo } from "../../../ChangePortalOwnerDialog/StyledDialog";
import styled from "styled-components";
import Avatar from "@docspace/components/avatar";
import Text from "@docspace/components/text";
import CatalogSpamIcon from "PUBLIC_DIR/images/catalog.spam.react.svg";
import commonIconsStyles from "@docspace/components/utils/common-icons-style";

const StyledCatalogSpamIcon = styled(CatalogSpamIcon)`
  ${commonIconsStyles}
  path {
    fill: #f21c0e;
  }

  padding-left: 8px;
`;

const AccountInfo = ({ user }) => {
  const firstLetterToUppercase = (str) => {
    return str[0].toUpperCase() + str.slice(1);
  };

  return (
    <StyledOwnerInfo>
      <Avatar
        className="avatar"
        role="user"
        source={user.avatar}
        size={"big"}
        hideRoleIcon
      />
      <div className="info">
        <div className="avatar-name">
          <Text className="display-name" noSelect title={user.displayName}>
            {user.displayName}
          </Text>
          {user.statusType === "disabled" && (
            <StyledCatalogSpamIcon size="small" />
          )}
        </div>

        <Text className="status" noSelect>
          {firstLetterToUppercase(user.statusType)}
        </Text>
      </div>
    </StyledOwnerInfo>
  );
};

export default AccountInfo;
