import { StyledAvailableList } from "../../../ChangePortalOwnerDialog/StyledDialog";
import Link from "@docspace/components/link";
import Text from "@docspace/components/text";

const Description = ({ t, dataReassignmentUrl }) => {
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
          target="_blank"
          isHovered
          fontWeight={600}
          style={{ textDecoration: "underline" }}
          href={dataReassignmentUrl}
        >
          {t("DataReassignmentDialog:MoreAboutDataTransfer")}
        </Link>
      </StyledAvailableList>
    </>
  );
};

export default Description;
