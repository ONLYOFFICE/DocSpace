import AccessNoneIcon from "../../../public/images/access.none.react.svg?url";
import CheckRoundIcon from "../../../public/images/check.react.svg?url";
import CommentIcon from "../../../public/images/access.comment.react.svg?url";
import CrownIcon from "../../../public/images/access.edit.react.svg?url";
import EyeIcon from "../../../public/images/eye.react.svg?url";
import PencilIcon from "../../../public/images/access.edit.react.svg?url";
import ReviewIcon from "../../../public/images/access.review.react.svg?url";

export const data = [
  {
    key: "key1",
    label: "Room administrator",
    description: `Administration of rooms, archiving of rooms, inviting and managing users in rooms.`,
    icon: CrownIcon,
    quota: "free",
    color: "#20D21F",
  },
  {
    key: "key2",
    label: "Full access",
    description: `Edit, upload, create, view, download, delete files and folders.`,
    icon: CheckRoundIcon,
    quota: "paid",
    color: "#EDC409",
  },

  { key: "key3", isSeparator: true },
  {
    key: "key4",
    label: "Editing",
    description: `Editing, viewing, downloading files and folders, filling out forms.`,
    icon: PencilIcon,
  },
  {
    key: "key5",
    label: "Review",
    description: `Reviewing, viewing, downloading files and folders, filling out forms.`,
    icon: ReviewIcon,
  },
  {
    key: "key6",
    label: "Comment",
    description: `Commenting on files, viewing, downloading files and folders, filling out forms.`,
    icon: CommentIcon,
  },
  {
    key: "key7",
    label: "Read only",
    description: `Viewing, downloading files and folders, filling out forms.`,
    icon: EyeIcon,
  },
  {
    key: "key8",
    label: "Deny access",
    description: "",
    icon: AccessNoneIcon,
  },
];
