import Text from "@docspace/components/text";
import styled from "styled-components";
import { ColorTheme, ThemeType } from "@docspace/components/ColorTheme";

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
`;
const Progress = ({ fromUser, toUser, isReassignCurrentUser, percent }) => {
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
            {percent < 50 ? "In progress" : "All data transferred"}
          </Text>
          <Text noSelect>
            {percent < 50
              ? "Pending..."
              : percent < 100
              ? "In progress"
              : "All data transferred"}
          </Text>
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
