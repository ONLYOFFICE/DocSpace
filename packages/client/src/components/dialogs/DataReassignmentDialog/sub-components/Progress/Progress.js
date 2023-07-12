import Text from "@docspace/components/text";
import styled from "styled-components";
import { ColorTheme, ThemeType } from "@docspace/components/ColorTheme";

const StyledProgress = styled.div`
  .description {
    line-height: 20px;
    padding-bottom: 16px;
  }

  .user-name {
    font-weight: 600;
  }

  .progress {
    margin: 16px 0;
  }

  .progress-status {
    display: flex;
    align-items: center;
    gap: 40px;
  }
`;
const Progress = ({ fromUser, toUser, isReassignCurrentUser, percent }) => {
  return (
    <StyledProgress className="description">
      <Text noSelect>
        The process of data reassignment from user
        <span className="user-name"> {fromUser} </span> to user
        <span className="user-name"> {toUser} </span>
        {isReassignCurrentUser && " (You) "}
        has started. Please note that it may take a considerable time.
      </Text>

      <div className="progress-status">
        <Text noSelect>Rooms</Text>
        <div>{percent < 50 ? "In progress" : "All data transferred"} </div>
      </div>

      <div className="progress-status">
        <Text noSelect>Documents</Text>
        <div>
          {percent < 50
            ? "Pending..."
            : percent < 100
            ? "In progress"
            : "All data transferred"}
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
