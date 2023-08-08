import Text from "@docspace/components/text";
import styled from "styled-components";
import { ColorTheme, ThemeType } from "@docspace/components/ColorTheme";
import Loader from "@docspace/components/loader";
import CheckIcon from "PUBLIC_DIR/images/check.edit.react.svg";
import commonIconsStyles from "@docspace/components/utils/common-icons-style";
import { Base } from "@docspace/components/themes";
import { withTranslation, Trans } from "react-i18next";

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

  .user {
    display: inline;
    font-weight: 600;
  }
`;
const Progress = ({ fromUser, toUser, isReassignCurrentUser, percent, t }) => {
  const inProgressNode = (
    <div className="in-progress">
      <Loader size="20px" type="track" />
      <Text>{t("DataReassignmentDialog:InProgress")}</Text>
    </div>
  );

  const allDataTransferredNode = (
    <div className="all-data-transferred">
      <StyledCheckIcon size="medium" />
      <Text>{t("DataReassignmentDialog:AllDataTransferred")}</Text>
    </div>
  );

  const you = `${`(` + t("Common:You") + `)`}`;

  const reassigningDataToItself = (
    <Trans
      i18nKey="ReassigningDataToItself"
      ns="DataReassignmentDialog"
      t={t}
      fromUser={fromUser}
      toUser={toUser}
      you={you}
    >
      The process of data reassignment from user
      <div className="user"> {{ fromUser }}</div> to user
      <div className="user"> {{ toUser }}</div>
      <div className="user"> {{ you }}</div> has started. Please note that it
      may take a considerable time.
    </Trans>
  );

  const reassigningDataToAnother = (
    <Trans
      i18nKey="ReassigningDataToAnother"
      ns="DataReassignmentDialog"
      t={t}
      fromUser={fromUser}
      toUser={toUser}
    >
      The process of data reassignment from user
      <div className="user"> {{ fromUser }}</div> to user
      <div className="user"> {{ toUser }}</div> has started. Please note that it
      may take a considerable time.
    </Trans>
  );

  const reassigningDataStart = isReassignCurrentUser
    ? reassigningDataToItself
    : reassigningDataToAnother;

  return (
    <StyledProgress>
      <div>{reassigningDataStart}</div>

      <div className="progress-container">
        <div className="progress-section">
          <Text className="progress-section-text" noSelect>
            {t("Common:Rooms")}
          </Text>
          <Text className="progress-section-text" noSelect>
            {t("Common:Documents")}
          </Text>
        </div>

        <div className="progress-status">
          {percent < 50 ? inProgressNode : allDataTransferredNode}

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
      />

      <Text className="description" noSelect>
        {t("DataReassignmentDialog:ProcessComplete")}
      </Text>
    </StyledProgress>
  );
};

export default withTranslation(["Common, DataReassignmentDialog"])(Progress);
