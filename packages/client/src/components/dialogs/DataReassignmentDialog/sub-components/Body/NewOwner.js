import {
  StyledPeopleSelectorInfo,
  StyledPeopleSelector,
  StyledSelectedOwnerContainer,
  StyledSelectedOwner,
} from "../../../ChangePortalOwnerDialog/StyledDialog";
import Link from "@docspace/components/link";
import Text from "@docspace/components/text";
import SelectorAddButton from "@docspace/components/selector-add-button";

const ChoiceNewOwner = ({
  t,
  selectedUser,
  currentColorScheme,
  onTogglePeopleSelector,
}) => {
  if (selectedUser)
    return (
      <StyledSelectedOwnerContainer>
        <StyledSelectedOwner currentColorScheme={currentColorScheme}>
          <Text className="text">
            {selectedUser.displayName
              ? selectedUser.displayName
              : selectedUser.label}
          </Text>
        </StyledSelectedOwner>

        <Link
          type={"action"}
          isHovered
          fontWeight={600}
          onClick={onTogglePeopleSelector}
        >
          {t("ChangePortalOwner:ChangeUser")}
        </Link>
      </StyledSelectedOwnerContainer>
    );

  return (
    <StyledPeopleSelector>
      <SelectorAddButton
        className="selector-add-button"
        onClick={onTogglePeopleSelector}
      />
      <Text className="label" noSelect title={t("Translations:ChooseFromList")}>
        {t("Translations:ChooseFromList")}
      </Text>
    </StyledPeopleSelector>
  );
};

const NewOwner = ({
  t,
  selectedUser,
  currentColorScheme,
  onTogglePeopleSelector,
}) => {
  return (
    <>
      <StyledPeopleSelectorInfo>
        <Text className="new-owner" noSelect>
          {t("DataReassignmentDialog:NewDataOwner")}
        </Text>
        <Text className="description" noSelect>
          {t("DataReassignmentDialog:UserToWhomTheDataWillBeTransferred")}
        </Text>
      </StyledPeopleSelectorInfo>

      <ChoiceNewOwner
        t={t}
        selectedUser={selectedUser}
        currentColorScheme={currentColorScheme}
        onTogglePeopleSelector={onTogglePeopleSelector}
      />
    </>
  );
};

export default NewOwner;
