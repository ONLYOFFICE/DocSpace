interface CronProps {
  value?: string;
  setValue: (value: string) => void;
  onError?: (error?: Error) => void;
}

export default CronProps;
