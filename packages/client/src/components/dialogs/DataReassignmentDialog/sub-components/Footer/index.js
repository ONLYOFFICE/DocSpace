import Checkbox from "@docspace/components/checkbox";
import Button from "@docspace/components/button";
import Text from "@docspace/components/text";
import { StyledFooterWrapper } from "../../../ChangePortalOwnerDialog/StyledDialog";

const Footer = ({
  t,
  showProgress,
  isDeleteProfile,
  onToggleDeleteProfile,
  selectedUser,
  onReassign,
  percent,
  isAbortTransfer,
  onClose,
  onTerminate,
  onStartAgain,
}) => {
  if (showProgress) {
    return (
      <StyledFooterWrapper>
        <div className="button-wrapper">
          <Button
            label={
              isAbortTransfer && percent !== 100
                ? t("DataReassignmentDialog:StartTransferAgain")
                : percent === 100
                ? t("Common:OkButton")
                : t("DataReassignmentDialog:AbortTransfer")
            }
            size="normal"
            scale
            onClick={
              isAbortTransfer && percent !== 100
                ? onStartAgain
                : percent === 100
                ? onClose
                : onTerminate
            }
          />
        </div>
      </StyledFooterWrapper>
    );
  }

  return (
    <StyledFooterWrapper>
      <div className="delete-profile-container">
        <Checkbox
          className="delete-profile-checkbox"
          isChecked={isDeleteProfile}
          onClick={onToggleDeleteProfile}
        />
        <Text className="info" noSelect>
          {t("DataReassignmentDialog:DeleteProfileIsFinished")}
        </Text>
      </div>

      <div className="button-wrapper">
        <Button
          label={t("DataReassignmentDialog:Reassign")}
          size="normal"
          primary
          scale
          isDisabled={!selectedUser}
          onClick={onReassign}
        />

        <Button
          label={t("Common:CancelButton")}
          size="normal"
          scale
          onClick={onClose}
        />
      </div>
    </StyledFooterWrapper>
  );
};

export default Footer;
