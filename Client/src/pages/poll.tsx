import { useParams } from 'react-router-dom';

export default function Poll() {
    const { pollId } = useParams();
    console.log(pollId);

    return <>poll</>;
}
