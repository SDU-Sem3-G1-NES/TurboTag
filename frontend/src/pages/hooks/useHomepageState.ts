import { useEffect, useState } from 'react'
import { LessonClient, LessonDto, LessonFilter } from '../../api/apiClient'

export const useHomePageState = () => {
  const lessonClient = new LessonClient()

  const [ownerLessons, setOwnerLessons] = useState<LessonDto[]>([])
  const [search, setSearch] = useState<string>('')
  const [loading, setLoading] = useState<boolean>(true)
  const [filter, setFilter] = useState<LessonFilter>(
    new LessonFilter({
      ownerId: parseInt(localStorage.getItem('userId') ?? '0', 10),
      searchText: ''
    })
  )

  const loadLessons = async () => {
    try {
      setLoading(true)
      const data = await lessonClient.getAllLessons(filter)
      setOwnerLessons(Array.isArray(data) ? data : data.items || [])
    } catch (error) {
      console.error('Error fetching lessons:', error)
    } finally {
      setLoading(false)
    }
  }

  const setFilterProperty = <K extends keyof LessonFilter>(key: K, value: LessonFilter[K]) => {
    setFilter((prev: LessonFilter) => {
      const newFilter = new LessonFilter({ ...prev, [key]: value })
      return newFilter
    })
  }

  const handleSearch = (text: string) => {
    setFilterProperty('searchText', text)
  }

  useEffect(() => {
    loadLessons()
  }, [filter])

  // Debounced effect
  useEffect(() => {
    const delay = setTimeout(() => {
      handleSearch(search)
    }, 500)
    return () => clearTimeout(delay)
  }, [search])

  return {
    ownerLessons,
    loading,
    search,
    setSearch,
    handleSearch,
    filter,
    setFilterProperty,
    refresh: loadLessons
  }
}
