import {
  AccessNoneIcon,
  CheckRoundIcon,
  CommentIcon,
  CrownIcon,
  EyeIcon,
  PencilIcon,
  ReviewIcon,
} from "./svg";

export const data = [
  {
    key: "key1",
    title: "Room administrator",
    description:
      "Администрирование комнат, архивирование комнат, приглашение и управление пользователями в комнатах.",
    icon: CrownIcon,
  },
  {
    key: "key2",
    title: "Full access",
    description:
      "Редактирование, загрузка, создание, просмотр, скачивание, удаление файлов и папок",
    icon: CheckRoundIcon,
  },

  { key: "key3", isSeparator: true },
  {
    key: "key4",
    title: "Editing",
    description:
      "Редактирование, просмотр, скачивание файлов и папок, заполнение форм",
    icon: PencilIcon,
  },
  {
    key: "key5",
    title: "Review",
    description:
      "Рецензирование, просмотр, скачивание файлов и папок, заполнение форм",
    icon: ReviewIcon,
  },
  {
    key: "key6",
    title: "Comment",
    description:
      "Комментирование файлов, просмотр, скачивание файлов и папок, заполнение форм",
    icon: CommentIcon,
  },
  {
    key: "key7",
    title: "Read only",
    description: "Просмотр, скачивание файлов и папок, заполнение форм",
    icon: EyeIcon,
  },
  {
    key: "key8",
    title: "Deny access",
    description: "",
    icon: AccessNoneIcon,
  },
];

export const quota = [
  {
    key: "key1",
    accessRightKey: "key1",
    quota: "free",
    color: "#20D21F",
  },
  {
    key: "key2",
    accessRightKey: "key2",
    quota: "paid",
    color: "#EDC409",
  },
];
