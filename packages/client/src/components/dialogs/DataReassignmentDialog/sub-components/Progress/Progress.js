import Text from "@docspace/components/text";
import styled from "styled-components";
import { ColorTheme, ThemeType } from "@docspace/components/ColorTheme";
import Loader from "@docspace/components/loader";
import CheckIcon from "PUBLIC_DIR/images/check.edit.react.svg";
import commonIconsStyles from "@docspace/components/utils/common-icons-style";
import { Base } from "@docspace/components/themes";

const StyledCheckIcon = styled(CheckIcon)`
  ${commonIconsStyles}
  path {
    fill: rgba(53, 173, 23, 1) !important;
  }
`;

StyledCheckIcon.defaultProps = { theme: Base };

const StyledProgress = styled.div`
  display: flex;
  flex-direction: column;
  gap: 16px;

  .description {
    line-height: 20px;
  }

  .user-name {
    font-weight: 600;
  }

  .progress {
    margin: 16px 0;
  }

  .progress-container {
    display: flex;
    gap: 16px;
  }

  .progress-section {
    display: flex;
    flex-direction: column;
    gap: 8px;
  }

  .progress-section-text {
    font-size: 14px;
    font-weight: 600;
    line-height: 16px;
  }

  .progress-status {
    display: flex;
    flex-direction: column;
    gap: 4px;
  }

  .in-progress {
    display: flex;
    gap: 4px;
  }

  .all-data-transferred {
    display: flex;
    gap: 6px;
  }

  .check-icon {
    width: 16px;
  }
`;
const Progress = ({ fromUser, toUser, isReassignCurrentUser, percent }) => {
  const inProgressNode = (
    <div className="in-progress">
      <Loader size="20px" type="track" />
      <Text>In progress</Text>
    </div>
  );

  const allDataTransferredNode = (
    <div className="all-data-transferred">
      <StyledCheckIcon size="medium" />
      <Text>All data transferred</Text>
    </div>
  );

  console.log("percent", percent);
  return (
    <StyledProgress>
      <Text noSelect>
        The process of data reassignment from user
        <span className="user-name"> {fromUser} </span> to user
        <span className="user-name"> {toUser} </span>
        {isReassignCurrentUser && " (You) "}
        has started. Please note that it may take a considerable time.
      </Text>

      <div className="progress-container">
        <div className="progress-section">
          <Text className="progress-section-text" noSelect>
            Rooms
          </Text>
          <Text className="progress-section-text" noSelect>
            Documents
          </Text>
        </div>

        <div className="progress-status">
          <Text noSelect>
            {percent < 50 ? inProgressNode : allDataTransferredNode}
          </Text>

          {percent < 50 ? (
            <Text noSelect>Pending...</Text>
          ) : percent < 100 ? (
            inProgressNode
          ) : (
            allDataTransferredNode
          )}
        </div>
      </div>

      <ColorTheme
        className="progress"
        themeId={ThemeType.MobileProgressBar}
        uploadPercent={percent}
        //   error={error}
      />

      <Text className="description" noSelect>
        You can close this page. When the process is completed, the responsible
        administrator will be notified by email.
      </Text>
    </StyledProgress>
  );
};

export default Progress;
