import { useParams } from 'react-router-dom';

export default function Poll() {
    const id = "sdfa"
    localStorage.setItem('redirect', `/polls/${id}`)
    const { pollId } = useParams();
    console.log(pollId);

    return <>poll</>;
}
