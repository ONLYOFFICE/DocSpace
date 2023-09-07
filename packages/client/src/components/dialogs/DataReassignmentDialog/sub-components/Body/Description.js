import { StyledAvailableList } from "../../../ChangePortalOwnerDialog/StyledDialog";
import Link from "@docspace/components/link";
import Text from "@docspace/components/text";

const Description = ({ t }) => {
  return (
    <>
      <StyledAvailableList className="list-container">
        <Text className="list-item" noSelect>
          {t("DataReassignmentDialog:DescriptionDataReassignment")}
        </Text>
        <Text className="list-item" noSelect>
          {t("DataReassignmentDialog:NoteDataReassignment")}
        </Text>

        <Link
          type={"action"}
          isHovered
          fontWeight={600}
          style={{ textDecoration: "underline" }}
        >
          {t("DataReassignmentDialog:MoreAboutDataTransfer")}
        </Link>
      </StyledAvailableList>
    </>
  );
};

export default Description;
